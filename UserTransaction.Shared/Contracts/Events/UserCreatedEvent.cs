namespace UserTransaction.Shared.Contracts.Events;

public record UserCreatedEvent
{
    public Guid CorrelationId { get; init; }
    public Guid UserId { get; init; }
}