namespace UserTransaction.Shared.Contracts.Commands;

public record CreateUserCommand
{
    public Guid CorrelationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
}