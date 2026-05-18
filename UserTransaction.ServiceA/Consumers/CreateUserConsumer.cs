namespace UserTransaction.ServiceA.Consumers;

using MassTransit;
using Shared.Contracts.Commands;
using Shared.Contracts.Events;
using UserTransaction.ServiceC.Protos;

public class CreateUserConsumer(UserService.UserServiceClient grpcClient) : IConsumer<CreateUserCommand>
{
    public async Task Consume(ConsumeContext<CreateUserCommand> context)
    {
        try
        {
            var reply = await grpcClient.CreateUserAsync(new CreateUserRequest
            {
                Email = context.Message.Email,
                Username = context.Message.Username
            });

            if (!reply.Success)
            {
                await context.Publish(new PersistenceFailedEvent
                {
                    CorrelationId = context.Message.CorrelationId,
                    Error = $"Service C refused creation: {reply.Error}"
                });
                return;
            }

            await context.Publish(new UserCreatedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                UserId = Guid.Parse(reply.UserId)
            });
        }
        catch (Exception ex)
        {
            await context.Publish(new PersistenceFailedEvent
            {
                CorrelationId = context.Message.CorrelationId,
                Error = ex.Message
            });
        }
    }
}