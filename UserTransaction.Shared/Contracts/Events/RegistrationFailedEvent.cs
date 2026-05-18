namespace UserTransaction.Shared.Contracts.Events;

public record RegistrationFailedEvent
{
    public Guid CorrelationId { get; init; }
    public string[] Errors { get; init; } = [];
}