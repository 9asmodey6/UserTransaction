namespace UserTransaction.Shared.Contracts.Commands;

public record ReleaseUsernameCommand
{
    public Guid CorrelationId { get; init; }
    public string Username { get; init; } = string.Empty;
}