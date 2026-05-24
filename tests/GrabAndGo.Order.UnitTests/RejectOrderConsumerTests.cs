using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Order.Application.Consumers;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;
using GrabAndGo.Order.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace GrabAndGo.Order.UnitTests;

public class RejectOrderConsumerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ILogger<RejectOrderConsumer>> _loggerMock;
    private readonly RejectOrderConsumer _consumer;

    public RejectOrderConsumerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<RejectOrderConsumer>>();
        _consumer = new RejectOrderConsumer(_orderRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_Should_UpdateStatusToRejected_When_NotProcessed()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var order = new Domain.Entities.Order { Id = orderId, Status = OrderStatus.NEW };

        var contextMock = new Mock<ConsumeContext<RejectOrderCommand>>();
        contextMock.Setup(c => c.Message).Returns(new RejectOrderCommand(orderId));
        contextMock.Setup(c => c.MessageId).Returns(messageId);

        _orderRepositoryMock.Setup(r => r.IsInboxMessageProcessedAsync(messageId, It.IsAny<string>()))
            .ReturnsAsync(false);
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        Assert.Equal(OrderStatus.REJECTED, order.Status);
        _orderRepositoryMock.Verify(r => r.AddInboxMessageAsync(It.IsAny<InboxMessage>()), Times.Once);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(order), Times.Once);
        _orderRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }
}
