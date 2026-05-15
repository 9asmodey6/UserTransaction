namespace UserTransaction.Shared.Contracts;

public record LogUserCommand(
    Guid CorrelationId,
    string Email);