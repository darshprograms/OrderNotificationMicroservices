using OrderService.Domain.Enums;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public int CustomerId { get; set; }
        public List<OrderItem> Items { get; set; } = new();
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public OrderStatus Status { get; set; } = OrderStatus.Pending;
    }
}
