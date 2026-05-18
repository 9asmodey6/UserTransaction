namespace UserTransaction.Shared.Contracts.Events;

public record ValidationFailedEvent
{
    public Guid CorrelationId { get; init; }
    public string[] Errors { get; init; } = [];
}
