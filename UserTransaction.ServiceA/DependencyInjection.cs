namespace UserTransaction.ServiceA;

using Consumers;
using MassTransit;
using Shared.Options;
using UserTransaction.ServiceC.Protos;

public static class DependencyInjection
{
    public static IServiceCollection RegisterMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitOptions = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()
                         ?? new RabbitMqSettings();
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<LogUserConsumer>();
            
            x.UsingRabbitMq((context, cfg) =>
            {
                cfg.Host(rabbitOptions.Host, rabbitOptions.VirtualHost, h =>
                {
                    h.Username(rabbitOptions.Username);
                    h.Password(rabbitOptions.Password);
                });
                cfg.ConfigureEndpoints(context);
            });
        });
        
        return services;
    }

    public static IServiceCollection RegisterGrpcClients(this IServiceCollection services, IConfiguration configuration)
    {
        var grpcOptions = configuration.GetSection(GrpcSettings.SectionName).Get<GrpcSettings>()
            ?? throw new InvalidOperationException("Grpc settings not found");

        services.AddGrpcClient<UserService.UserServiceClient>(o =>
        {
            o.Address = new Uri(grpcOptions.ServiceCAddress);
        });

        return services;
    }

    public static IServiceCollection ApplyConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(
            configuration.GetSection(RabbitMqSettings.SectionName));

        services.Configure<GrpcSettings>(
            configuration.GetSection(GrpcSettings.SectionName));

        return services;
    }
}