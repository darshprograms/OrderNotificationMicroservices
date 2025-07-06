using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IOrderRepository _repository;

        public CreateOrderHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public async Task<Order> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
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

            // ✅ Save the new order
            return await _repository.CreateOrderAsync(newOrder);
        }
    }
}
