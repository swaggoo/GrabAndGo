using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Order.Application.Saga;
using MassTransit;
using MassTransit.Testing;
using Microsoft.Extensions.DependencyInjection;

namespace GrabAndGo.Order.UnitTests;

public class OrderStateMachineTests
{
    [Fact]
    public async Task OrderSubmitted_Should_TransitionToSubmitted_And_PublishReserveStock()
    {
        // Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<OrderStateMachine, OrderStateData>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Act
        await harness.Bus.Publish(new OrderSubmittedEvent(
            orderId,
            productId,
            "user-1",
            100.0m
        ));

        // Assert
        Assert.True(await harness.Consumed.Any<OrderSubmittedEvent>());
        
        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderStateData>();
        
        Assert.True(await sagaHarness.Consumed.Any<OrderSubmittedEvent>());
        
        var sagaCreatedId = await sagaHarness.Created.Any(x => x.CorrelationId == orderId);
        Assert.NotNull(sagaCreatedId);

        var instance = sagaHarness.Created.ContainsInState(orderId, sagaHarness.StateMachine, sagaHarness.StateMachine.Submitted);
        Assert.NotNull(instance);

        Assert.True(await harness.Published.Any<ReserveProductStockCommand>());
    }

    [Fact]
    public async Task ProductStockReserved_Should_TransitionToFinal_And_AcceptOrder()
    {
        // Arrange
        await using var provider = new ServiceCollection()
            .AddMassTransitTestHarness(x =>
            {
                x.AddSagaStateMachine<OrderStateMachine, OrderStateData>();
            })
            .BuildServiceProvider(true);

        var harness = provider.GetRequiredService<ITestHarness>();
        await harness.Start();

        var orderId = Guid.NewGuid();
        var productId = Guid.NewGuid();

        // Start Saga
        await harness.Bus.Publish(new OrderSubmittedEvent(orderId, productId, "user-1", 100.0m));
        
        var sagaHarness = harness.GetSagaStateMachineHarness<OrderStateMachine, OrderStateData>();
        var sagaCreatedId = await sagaHarness.Created.Any(x => x.CorrelationId == orderId);
        Assert.NotNull(sagaCreatedId);

        // Act: Simulate stock reserved
        await harness.Bus.Publish(new ProductStockReservedEvent(orderId, productId));

        // Assert
        Assert.True(await sagaHarness.Consumed.Any<ProductStockReservedEvent>());
        
        // Should be finalized/completed
        var notExistsId = await sagaHarness.NotExists(orderId);
        Assert.Null(notExistsId);
        
        var acceptOrderPublishedId = await harness.Published.Any<AcceptOrderCommand>(x => x.Context.Message.OrderId == orderId);
        Assert.NotNull(acceptOrderPublishedId);
    }
}
