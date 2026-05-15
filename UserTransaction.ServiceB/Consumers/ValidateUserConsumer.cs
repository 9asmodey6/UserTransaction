namespace UserTransaction.ServiceB.Consumers;

using FluentValidation;
using MassTransit;
using Shared.Contracts;
using Shared.Models;

public class ValidateUserConsumer(IValidator<ValidateUserCommand> validator) : IConsumer<ValidateUserCommand>
{
    public async Task Consume(ConsumeContext<ValidateUserCommand> context)
    {
        var result = await validator.ValidateAsync(context.Message);

        var response = result.IsValid
            ? Result.Success()
            : Result.Failure(result.Errors.Select(e => e.ErrorMessage).ToArray());

        await context.RespondAsync(response);
    }
}