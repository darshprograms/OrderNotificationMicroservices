using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Enums;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderHandler : IRequestHandler<CreateOrderCommand, Order>
    {
        private readonly IOrderRepository _repository;
        private readonly INotificationService _notifier;
        private readonly IMessagingService _messaging;

        public CreateOrderHandler(IOrderRepository repository, INotificationService notifier, IMessagingService messaging)
        {
            _repository = repository;
            _notifier = notifier;
            _messaging = messaging;
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
            var createdOrder = await _repository.CreateOrderAsync(newOrder);

            // 🔔 Notify
            await _notifier.NotifyOrderCreatedAsync(createdOrder);

            await _messaging.PublishOrderCreatedAsync(createdOrder.OrderId, createdOrder.Timestamp);

            return createdOrder;
        }
    }
}
