using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        public Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
        {
            var newOrder = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = request.CustomerId,
                Timestamp = DateTime.UtcNow,
                Status = OrderStatus.Pending,
                Items = request.Items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };

            // Later we will: Save to DB, Notify, Publish Kafka etc.
            return Task.FromResult(newOrder);
        }
    }
}

