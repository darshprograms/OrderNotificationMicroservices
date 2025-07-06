using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface IOrderRepository
    {
        Task<Order> CreateOrderAsync(Order order);
        Task<Order?> GetOrderByIdAsync(Guid orderId);
    }
}
