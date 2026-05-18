namespace UserTransaction.ServiceA.Features.Users.Register;

using MassTransit;
using Shared.Contracts.Commands;
using Shared.Contracts.Events;
using Shared.Models;

public static class RegisterUserEndpoint
{
    public static void MapRegisterUserEndpoint(this IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", HandleAsync)
            .WithName("RegisterUser")
            .Produces<Result<Guid>>(StatusCodes.Status201Created)
            .Produces<Result>(StatusCodes.Status400BadRequest)
            .Produces<Result>(StatusCodes.Status500InternalServerError);
    }

    private static async Task<IResult> HandleAsync(
        RegisterUserRequest request,
        IRequestClient<StartRegistrationCommand> client)
    {
        var correlationId = Guid.NewGuid();

        // Starting transaction
        var response = await client.GetResponse<UserCreatedEvent, RegistrationFailedEvent>(
            new StartRegistrationCommand
            {
                CorrelationId = correlationId,
                Email = request.Email,
                Username = request.Username
            });

        if (response.Is<UserCreatedEvent>(out var success))
        {
            return Results.Created($"/api/users/{success.Message.UserId}",
                Result<Guid>.Success(success.Message.UserId));
        }

        if (response.Is<RegistrationFailedEvent>(out var fail))
        {
            return Results.BadRequest(Result.Failure(fail.Message.Errors));
        }

        return Results.Problem("Unknown error occurred during registration.");
    }
}