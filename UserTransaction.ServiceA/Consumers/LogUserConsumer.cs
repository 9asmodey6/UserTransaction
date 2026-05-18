namespace UserTransaction.ServiceA.Consumers;

using MassTransit;
using Shared.Contracts.Commands;
using Shared.Models;

public class LogUserConsumer(ILogger<LogUserConsumer> logger) : IConsumer<LogUserCommand>
{
    public async Task Consume(ConsumeContext<LogUserCommand> context)
    {
        try
        {
            logger.LogInformation("[{CorrelationId}] Registration started for {Email}",
                context.Message.CorrelationId, context.Message.Email);

            await context.RespondAsync(Result.Success());
        }
        catch (Exception ex)
        {
            await context.RespondAsync(Result.Failure(ex.ToString())); 
        }
    }
}