using Grpc.Core;
using UserTransaction.ServiceC.Protos;

namespace UserTransaction.ServiceC.Services;

public class UserGrpcService : UserService.UserServiceBase
{
    private readonly ILogger<UserGrpcService> _logger;

    public UserGrpcService(ILogger<UserGrpcService> logger)
    {
        _logger = logger;
    }

    public override Task<CreateUserReply> CreateUser(CreateUserRequest request, ServerCallContext context)
    {
        _logger.LogInformation("Creating user via gRPC: Email={Email}, Username={Username}", request.Email, request.Username);
        try
        {
            var newUserId = Guid.NewGuid().ToString();
            
            return Task.FromResult(new CreateUserReply
            {
                Success = true,
                UserId = newUserId,
                Error = ""
            });
        }
        catch (Exception e)
        {
            return Task.FromResult(new CreateUserReply
            {
                Success = false,
                Error = e.Message
            });
        }
    }
}
