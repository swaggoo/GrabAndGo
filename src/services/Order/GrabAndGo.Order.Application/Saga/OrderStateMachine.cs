using GrabAndGo.BuildingBlocks.Events;
using MassTransit;

namespace GrabAndGo.Order.Application.Saga;

public class OrderStateMachine : MassTransitStateMachine<OrderStateData>
{
    public OrderStateMachine()
    {
        InstanceState(x => x.CurrentState);

        Event(() => OrderSubmitted, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ProductStockReserved, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ProductStockFailed, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => OrderCancelled, x => x.CorrelateById(context => context.Message.OrderId));
        Event(() => ProductStockReleased, x => x.CorrelateById(context => context.Message.OrderId));

        Initially(
            When(OrderSubmitted)
                .Then(context =>
                {
                    context.Saga.OrderId = context.Message.OrderId;
                    context.Saga.ProductId = context.Message.ProductId;
                    context.Saga.UserId = context.Message.UserId;
                    context.Saga.Created = DateTime.UtcNow;
                })
                .TransitionTo(Submitted)
                .Publish(context => new ReserveProductStockCommand(
                    context.Message.OrderId,
                    context.Message.ProductId,
                    1
                ))
        );

        During(Submitted,
            When(ProductStockReserved)
                .Then(context => context.Saga.Updated = DateTime.UtcNow)
                .TransitionTo(StockReserved)
                .Publish(context => new AcceptOrderCommand(context.Saga.OrderId))
                .Finalize(),
            When(ProductStockFailed)
                .Then(context => context.Saga.Updated = DateTime.UtcNow)
                .TransitionTo(StockReservationFailed)
                .Publish(context => new RejectOrderCommand(context.Saga.OrderId))
                .Finalize()
        );

        // Example of a manual cancellation or failure after reservation
        During(StockReserved,
            When(OrderCancelled) // Assuming we add this event
                .Then(context => context.Saga.Updated = DateTime.UtcNow)
                .TransitionTo(Compensating)
                .Publish(context => new ReleaseProductStockCommand(context.Saga.OrderId, context.Saga.ProductId, 1))
                .Publish(context => new RejectOrderCommand(context.Saga.OrderId))
        );

        During(Compensating,
            When(ProductStockReleased)
                .Then(context => context.Saga.Updated = DateTime.UtcNow)
                .Finalize()
        );

        SetCompletedWhenFinalized();
    }

    public State Submitted { get; private set; } = default!;
    public State StockReserved { get; private set; } = default!;
    public State StockReservationFailed { get; private set; } = default!;
    public State Compensating { get; private set; } = default!;

    public Event<OrderSubmittedEvent> OrderSubmitted { get; private set; } = default!;
    public Event<ProductStockReservedEvent> ProductStockReserved { get; private set; } = default!;
    public Event<ProductStockReservationFailedEvent> ProductStockFailed { get; private set; } = default!;
    public Event<ProductStockReleasedEvent> ProductStockReleased { get; private set; } = default!;
    public Event<OrderCancelledEvent> OrderCancelled { get; private set; } = default!;
}

public record OrderCancelledEvent(Guid OrderId);
