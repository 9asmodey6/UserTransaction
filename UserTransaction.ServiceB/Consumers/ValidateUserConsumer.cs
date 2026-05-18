namespace UserTransaction.ServiceB.Consumers;

using System.Collections.Concurrent;
using FluentValidation;
using MassTransit;
using Shared.Contracts.Commands;
using Shared.Contracts.Events;

public class ValidateUserConsumer(
    IValidator<ValidateUserCommand> validator,
    ConcurrentDictionary<string, bool> reservedUsernames) : IConsumer<ValidateUserCommand>
{
    public async Task Consume(ConsumeContext<ValidateUserCommand> context)
    {
        var result = await validator.ValidateAsync(context.Message);

        if (!result.IsValid)
        {
            await context.Publish(new ValidationFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Errors = result.Errors.Select(e => e.ErrorMessage).ToArray()
            });
            return;
        }

        // Try to reserve the username
        var isReserved = reservedUsernames.TryAdd(context.Message.Username.ToLower(), true);
        
        if (!isReserved)
        {
            await context.Publish(new ValidationFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Errors = ["Username is already taken."]
            });
            return;
        }

        await context.Publish(new UserValidatedEvent { CorrelationId = context.Message.CorrelationId });
    }
}