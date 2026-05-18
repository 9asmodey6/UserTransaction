namespace UserTransaction.ServiceA.Consumers;

using MassTransit;
using Shared.Contracts.Commands;
using Shared.Contracts.Events;

public class LogUserConsumer(ILogger<LogUserConsumer> logger) : IConsumer<LogUserCommand>
{
    public async Task Consume(ConsumeContext<LogUserCommand> context)
    {
        try
        {
            logger.LogInformation("[{CorrelationId}] Registration started for {Email}",
                context.Message.CorrelationId, context.Message.Email);

            await context.Publish(new UserLoggedEvent { CorrelationId = context.Message.CorrelationId });
        }
        catch (Exception ex)
        {
            await context.Publish(new LoggingFailedEvent { CorrelationId = context.Message.CorrelationId, Error = ex.ToString() });
        }
    }
}