namespace UserTransaction.Shared.Contracts.Events;

public record PersistenceFailedEvent
{
    public Guid CorrelationId { get; init; }
    public string Error { get; init; } = string.Empty;
}