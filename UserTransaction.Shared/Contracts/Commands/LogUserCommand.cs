namespace UserTransaction.Shared.Contracts.Commands;

public record LogUserCommand
{
    public Guid CorrelationId { get; init; }
    public string Email { get; init; } = string.Empty;
}