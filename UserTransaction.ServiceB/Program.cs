using UserTransaction.ServiceB;

var builder = Host.CreateApplicationBuilder(args);
builder.Services
    .ApplyConfigurations(builder.Configuration)
    .RegisterMassTransit(builder.Configuration)
    .RegisterValidators();


var host = builder.Build();
host.Run();