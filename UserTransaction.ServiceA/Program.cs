using UserTransaction.ServiceA;
using UserTransaction.ServiceA.Features.Users.Register;

var builder = WebApplication.CreateBuilder(args);

builder.Services
    .ApplyConfigurations(builder.Configuration)
    .RegisterMassTransit(builder.Configuration)
    .RegisterGrpcClients(builder.Configuration)
    .AddEndpointsApiExplorer()
    .AddSwaggerGen(); 

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapRegisterUserEndpoint();

app.Run();