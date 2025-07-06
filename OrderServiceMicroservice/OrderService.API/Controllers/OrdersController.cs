using MediatR;
using Microsoft.AspNetCore.Mvc;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Queries.GetOrder;
using OrderService.Domain.Entities;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("orders")]
    public class OrdersController : ControllerBase
    {
        private readonly IMediator _mediator;

        public OrdersController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<ActionResult<Order>> CreateOrder([FromBody] CreateOrderCommand command)
        {
            var result = await _mediator.Send(command);
            return CreatedAtAction(nameof(CreateOrder), new { id = result.OrderId }, result);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Order>> GetOrder(Guid id)
        {
            var order = await _mediator.Send(new GetOrderQuery(id));
            return order is not null ? Ok(order) : NotFound();
        }
    }
}
