using Microsoft.AspNetCore.Mvc;
using OrderService.Domain.Entities;

namespace OrderService.API.Controllers
{
    [ApiController]
    [Route("notification")]
    public class NotificationServiceController : ControllerBase
    {
        [HttpPost]
        public IActionResult ReceiveNotification([FromBody] Order order)
        {
            Console.WriteLine($"📬 NotificationService received order: {order.OrderId}");
            return Ok(new { message = "Notification received." });
        }
    }
}
