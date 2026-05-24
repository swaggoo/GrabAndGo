using GrabAndGo.BuildingBlocks.Events;
using GrabAndGo.Order.Application.Consumers;
using GrabAndGo.Order.Domain.Entities;
using GrabAndGo.Order.Domain.Enums;
using GrabAndGo.Order.Domain.Repositories;
using MassTransit;
using Microsoft.Extensions.Logging;
using Moq;

namespace GrabAndGo.Order.UnitTests;

public class AcceptOrderConsumerTests
{
    private readonly Mock<IOrderRepository> _orderRepositoryMock;
    private readonly Mock<ILogger<AcceptOrderConsumer>> _loggerMock;
    private readonly AcceptOrderConsumer _consumer;

    public AcceptOrderConsumerTests()
    {
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _loggerMock = new Mock<ILogger<AcceptOrderConsumer>>();
        _consumer = new AcceptOrderConsumer(_orderRepositoryMock.Object, _loggerMock.Object);
    }

    [Fact]
    public async Task Consume_Should_UpdateStatus_When_NotProcessed()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var messageId = Guid.NewGuid();
        var order = new Domain.Entities.Order { Id = orderId, Status = OrderStatus.NEW };

        var contextMock = new Mock<ConsumeContext<AcceptOrderCommand>>();
        contextMock.Setup(c => c.Message).Returns(new AcceptOrderCommand(orderId));
        contextMock.Setup(c => c.MessageId).Returns(messageId);

        _orderRepositoryMock.Setup(r => r.IsInboxMessageProcessedAsync(messageId, It.IsAny<string>()))
            .ReturnsAsync(false);
        _orderRepositoryMock.Setup(r => r.GetByIdAsync(orderId))
            .ReturnsAsync(order);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        Assert.Equal(OrderStatus.ACCEPTED, order.Status);
        _orderRepositoryMock.Verify(r => r.AddInboxMessageAsync(It.Is<InboxMessage>(m => m.Id == messageId)), Times.Once);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(order), Times.Once);
        _orderRepositoryMock.Verify(r => r.SaveChangesAsync(), Times.Once);
    }

    [Fact]
    public async Task Consume_Should_DoNothing_When_AlreadyProcessed()
    {
        // Arrange
        var orderId = Guid.NewGuid();
        var messageId = Guid.NewGuid();

        var contextMock = new Mock<ConsumeContext<AcceptOrderCommand>>();
        contextMock.Setup(c => c.Message).Returns(new AcceptOrderCommand(orderId));
        contextMock.Setup(c => c.MessageId).Returns(messageId);

        _orderRepositoryMock.Setup(r => r.IsInboxMessageProcessedAsync(messageId, It.IsAny<string>()))
            .ReturnsAsync(true);

        // Act
        await _consumer.Consume(contextMock.Object);

        // Assert
        _orderRepositoryMock.Verify(r => r.GetByIdAsync(It.IsAny<Guid>()), Times.Never);
        _orderRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Domain.Entities.Order>()), Times.Never);
    }
}
