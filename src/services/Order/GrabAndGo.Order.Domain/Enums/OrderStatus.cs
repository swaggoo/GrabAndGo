namespace GrabAndGo.Order.Domain.Enums;

public enum OrderStatus
{
    NEW,
    ACCEPTED,
    PREPARING,
    READY_FOR_PICKUP,
    COMPLETED,
    CANCELLED,
    REJECTED,
    REFUNDED
}
