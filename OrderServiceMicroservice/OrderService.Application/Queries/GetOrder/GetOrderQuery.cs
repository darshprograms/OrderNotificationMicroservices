using MediatR;
using OrderService.Domain.Entities;

namespace OrderService.Application.Queries.GetOrder
{
    public record GetOrderQuery(Guid OrderId) : IRequest<Order?>;
}
