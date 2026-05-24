using MassTransit;

namespace GrabAndGo.Order.Application.Saga;

public class OrderStateData : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public string CurrentState { get; set; } = default!;
    
    public Guid OrderId { get; set; }
    public Guid ProductId { get; set; }
    public string UserId { get; set; } = default!;
    
    public DateTime Created { get; set; }
    public DateTime Updated { get; set; }
}
