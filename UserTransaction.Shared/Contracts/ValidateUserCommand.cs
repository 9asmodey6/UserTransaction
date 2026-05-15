namespace UserTransaction.Shared.Contracts;

public record ValidateUserCommand(
    Guid CorrelationId,
    string Email,
    string Username);