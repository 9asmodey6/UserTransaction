namespace UserTransaction.ServiceB.Consumers;

using System.Collections.Concurrent;
using MassTransit;
using Shared.Contracts.Commands;

public class ReleaseUsernameConsumer(
    ConcurrentDictionary<string, bool> reservedUsernames,
    ILogger<ReleaseUsernameConsumer> logger) : IConsumer<ReleaseUsernameCommand>
{
    public Task Consume(ConsumeContext<ReleaseUsernameCommand> context)
    {
        var username = context.Message.Username.ToLower();
        
        if (reservedUsernames.TryRemove(username, out _))
        {
            logger.LogInformation("[{CorrelationId}] Released reserved username {Username} (Compensation)", 
                context.Message.CorrelationId, username);
        }
        else
        {
            logger.LogWarning("[{CorrelationId}] Could not release username {Username} because it was not found.", 
                context.Message.CorrelationId, username);
        }

        return Task.CompletedTask;
    }
}