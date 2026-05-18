namespace UserTransaction.Shared.Contracts.Commands;

public record ValidateUserCommand
{
    public Guid CorrelationId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string Username { get; init; } = string.Empty;
}