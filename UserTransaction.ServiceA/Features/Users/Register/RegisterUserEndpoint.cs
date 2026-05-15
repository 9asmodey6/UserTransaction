namespace UserTransaction.ServiceA.Features.Users.Register;

using MassTransit;
using Microsoft.AspNetCore.Http.HttpResults;
using ServiceC.Protos;
using Shared.Contracts;
using Shared.Models;

public class RegisterUserEndpoint
{
    public static void MapEndpoint(IEndpointRouteBuilder app)
    {
        app.MapPost("/api/users", HandleAsync)
            .WithName("Register User")
            .WithSummary("Register new user");
    }

    private static async Task<IResult> HandleAsync(
        RegisterUserRequest request,
        IRequestClient<ValidateUserCommand> validateClient,
        IRequestClient<LogUserCommand> logClient,
        UserService.UserServiceClient client)
    {
        var correlationId = Guid.NewGuid();

        var validateTask = validateClient.GetResponse<Result>(
            new ValidateUserCommand(correlationId, request.Email, request.Username));
            
        var logTask = logClient.GetResponse<Result>(
            new LogUserCommand(correlationId, request.Email));
            
        await Task.WhenAll(validateTask, logTask);

        var validationResult = validateTask.Result.Message;
        var logResult = logTask.Result.Message;
        
        if (validationResult.IsFailure)
        {
            return Results.BadRequest(Result.Failure(validationResult.Errors));
        }

        if (logResult.IsFailure)
        {
            return Results.Problem("Logging failed");
        }
        
        var grpcReply = await client.CreateUserAsync(new CreateUserRequest
        {
            Email = request.Email,
            Username = request.Username
        });
        
        if (!grpcReply.Success)
        {
            return Results.Problem(grpcReply.Error);
        }
        
        var userId = Guid.Parse(grpcReply.UserId);
        return Results.Created($"/users/{userId}", Result<Guid>.Success(userId));
    }
}