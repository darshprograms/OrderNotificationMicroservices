using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using System.Collections.Concurrent;

namespace OrderService.Infrastructure.Repositories
{
    public class InMemoryOrderRepository : IOrderRepository
    {
        private static readonly ConcurrentDictionary<Guid, Order> _orders = new();

        public Task<Order> CreateOrderAsync(Order order)
        {
            _orders[order.OrderId] = order;
            return Task.FromResult(order);
        }

        public Task<Order?> GetOrderByIdAsync(Guid orderId)
        {
            _orders.TryGetValue(orderId, out var order);
            return Task.FromResult(order);
        }
    }
}
