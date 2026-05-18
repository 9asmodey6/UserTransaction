namespace UserTransaction.Shared.Contracts.Events;

public record LoggingFailedEvent
{
    public Guid CorrelationId { get; init; }
    public string Error { get; init; } = string.Empty;
}