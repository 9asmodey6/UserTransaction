namespace UserTransaction.Shared.Options;

public class GrpcSettings
{
    public const string SectionName = "Grpc";
    
    public string ServiceCAddress { get; init; } = string.Empty;
}