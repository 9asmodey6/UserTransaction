namespace UserTransaction.Shared.Contracts.Events;

public record UserLoggedEvent
{
    public Guid CorrelationId { get; init; }
}