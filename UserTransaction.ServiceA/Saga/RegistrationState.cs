namespace UserTransaction.ServiceA.Saga;

using MassTransit;

public class RegistrationState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = null!;
    
    public Uri? ResponseAddress { get; set; }
    public Guid? RequestId { get; set; }


    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    

    public bool IsValidated { get; set; }
    public bool IsLogged { get; set; }
    
    // Result
    public Guid? UserId { get; set; }
    public string[] Errors { get; set; } = [];
}