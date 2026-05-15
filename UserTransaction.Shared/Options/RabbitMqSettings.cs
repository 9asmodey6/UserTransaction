namespace UserTransaction.Shared.Options;

public class RabbitMqSettings
{
    public const string SectionName = "RabbitMq";
    
    public string Host { get; init; } = "localhost";
    public string VirtualHost { get; init; } = "/";
    public string Username { get; init; } = "guest";
    public string Password { get; init; } = "guest";
}