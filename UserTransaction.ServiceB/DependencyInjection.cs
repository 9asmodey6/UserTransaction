namespace UserTransaction.ServiceB;

using Consumers;
using FluentValidation;
using MassTransit;
using Shared.Options;
using Validators;

public static class DependencyInjection
{
    public static IServiceCollection RegisterMassTransit(this IServiceCollection services, IConfiguration configuration)
    {
        var rabbitOptions = configuration.GetSection(RabbitMqSettings.SectionName).Get<RabbitMqSettings>()
                            ?? new RabbitMqSettings();
        
        services.AddMassTransit(x =>
        {
            x.AddConsumer<ValidateUserConsumer>();
            
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

    public static IServiceCollection RegisterValidators(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<ValidateUserCommandValidator>();
        
        return services;
    }

    public static IServiceCollection ApplyConfigurations(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RabbitMqSettings>(
            configuration.GetSection(RabbitMqSettings.SectionName));

        return services;
    }
}