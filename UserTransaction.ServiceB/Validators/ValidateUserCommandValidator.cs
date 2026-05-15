namespace UserTransaction.ServiceB.Validators;

using FluentValidation;
using Shared.Contracts;

public class ValidateUserCommandValidator : AbstractValidator<ValidateUserCommand>
{
    public ValidateUserCommandValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Invalid email format");
        RuleFor(x => x.Username)
            .NotEmpty().WithMessage("Username is required")
            .MinimumLength(3).WithMessage("Username must be at least 3 characters")
            .MaximumLength(30).WithMessage("Username cannot exceed 30 characters");
    }
}