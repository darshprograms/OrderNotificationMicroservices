using MediatR;
using OrderService.Domain.Entities;
using OrderService.Application.Interfaces;

namespace OrderService.Application.Queries.GetOrder
{
    public class GetOrderHandler : IRequestHandler<GetOrderQuery, Order?>
    {
        private readonly IOrderRepository _repository;

        public GetOrderHandler(IOrderRepository repository)
        {
            _repository = repository;
        }

        public Task<Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            return _repository.GetOrderByIdAsync(request.OrderId);
        }
    }
}
