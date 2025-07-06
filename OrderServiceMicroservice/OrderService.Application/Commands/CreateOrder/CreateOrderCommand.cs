using MediatR;
using OrderService.Domain.Entities;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<Order>
    {
        public int CustomerId { get; set; }
        public List<OrderItemDto> Items { get; set; } = new();
    }

    public class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
