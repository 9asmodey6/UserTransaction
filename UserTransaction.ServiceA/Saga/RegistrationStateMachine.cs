namespace UserTransaction.ServiceA.Saga;

using MassTransit;
using Shared.Contracts.Commands;
using Shared.Contracts.Events;

public class RegistrationStateMachine : MassTransitStateMachine<RegistrationState>
{
    // States
    public State WaitingForBoth { get; private set; }
    public State Persisting { get; private set; }
    public State Completed { get; private set; }
    public State Failed { get; private set; }

    // Events
    public Event<StartRegistrationCommand> StartRegistration { get; private set; }
    // Validation
    public Event<UserValidatedEvent> UserValidated { get; private set; }
    public Event<ValidationFailedEvent> ValidationFailed { get; private set; }
    // Logging
    public Event<UserLoggedEvent> UserLogged { get; private set; }
    public Event<LoggingFailedEvent> LoggingFailed { get; private set; }
    // Persistence 
    public Event<UserCreatedEvent> UserCreated { get; private set; }
    public Event<PersistenceFailedEvent> PersistenceFailed { get; private set; }

    public RegistrationStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => StartRegistration, x =>
        {
            x.CorrelateById(m => m.Message.CorrelationId);
            x.InsertOnInitial = true;
        });

        Event(() => UserValidated, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => ValidationFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => UserLogged, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => LoggingFailed, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => UserCreated, x => x.CorrelateById(m => m.Message.CorrelationId));
        Event(() => PersistenceFailed, x => x.CorrelateById(m => m.Message.CorrelationId));

        Initially(
            When(StartRegistration)
                .Then(ctx =>
                {
                    ctx.Saga.Email = ctx.Message.Email;
                    ctx.Saga.Username = ctx.Message.Username;
                    ctx.Saga.RequestId = ctx.RequestId;
                    ctx.Saga.ResponseAddress = ctx.ResponseAddress;
                })
                .Publish(ctx => new ValidateUserCommand
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    Email = ctx.Message.Email,
                    Username = ctx.Message.Username
                })
                .Publish(ctx => new LogUserCommand
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    Email = ctx.Message.Email
                })
                .TransitionTo(WaitingForBoth)
        );

        During(WaitingForBoth,
            When(UserValidated)
                .Then(ctx => ctx.Saga.IsValidated = true)
                .If(ctx => BothCompleted(ctx.Saga),
                    binder => binder
                        .TransitionTo(Persisting)
                        .Publish(ctx => new CreateUserCommand
                        {
                            CorrelationId = ctx.Saga.CorrelationId,
                            Email = ctx.Saga.Email,
                            Username = ctx.Saga.Username
                        })),

            When(UserLogged)
                .Then(ctx => ctx.Saga.IsLogged = true)
                .If(ctx => BothCompleted(ctx.Saga),
                    binder => binder
                        .TransitionTo(Persisting)
                        .Publish(ctx => new CreateUserCommand
                        {
                            CorrelationId = ctx.Saga.CorrelationId,
                            Email = ctx.Saga.Email,
                            Username = ctx.Saga.Username
                        })),

            When(ValidationFailed)
                .Then(ctx => ctx.Saga.Errors = ctx.Message.Errors)
                .ThenAsync(async ctx => await SendResponseAsync(ctx, new RegistrationFailedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    Errors = ctx.Saga.Errors
                }))
                .TransitionTo(Failed)
                .Finalize(),

            When(LoggingFailed)
                .If(ctx => ctx.Saga.IsValidated,
                    binder => binder.Publish(ctx => new ReleaseUsernameCommand
                    {
                        CorrelationId = ctx.Saga.CorrelationId,
                        Username = ctx.Saga.Username
                    }))
                .ThenAsync(async ctx => await SendResponseAsync(ctx, new RegistrationFailedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    Errors = new[] { ctx.Message.Error }
                }))
                .TransitionTo(Failed)
                .Finalize()
        );

        During(Persisting,
            When(UserCreated)
                .Then(ctx => ctx.Saga.UserId = ctx.Message.UserId)
                .ThenAsync(async ctx => await SendResponseAsync(ctx, new UserCreatedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    UserId = ctx.Message.UserId
                }))
                .TransitionTo(Completed)
                .Finalize(),

            When(PersistenceFailed)
                .Publish(ctx => new ReleaseUsernameCommand
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    Username = ctx.Saga.Username
                })
                .ThenAsync(async ctx => await SendResponseAsync(ctx, new RegistrationFailedEvent
                {
                    CorrelationId = ctx.Saga.CorrelationId,
                    Errors = new[] { ctx.Message.Error }
                }))
                .TransitionTo(Failed)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    private static async Task SendResponseAsync<T>(BehaviorContext<RegistrationState, T> ctx, object response) where T : class
    {
        if (ctx.Saga.ResponseAddress != null)
        {
            var endpoint = await ctx.GetSendEndpoint(ctx.Saga.ResponseAddress);
            await endpoint.Send(response, sendCtx => sendCtx.RequestId = ctx.Saga.RequestId);
        }
    }

    private static bool BothCompleted(RegistrationState s) => s is { IsValidated: true, IsLogged: true };
}