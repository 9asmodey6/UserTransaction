namespace UserTransaction.Shared.Contracts.Events;

public record UserValidatedEvent
{
    public Guid CorrelationId { get; init; }
}