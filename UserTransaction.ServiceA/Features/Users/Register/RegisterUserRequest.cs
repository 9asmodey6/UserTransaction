namespace UserTransaction.ServiceA.Features.Users.Register;

public record RegisterUserRequest(
    string Email,
    string Username);