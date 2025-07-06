using OrderService.Domain.Entities;

namespace OrderService.Application.Interfaces
{
    public interface INotificationService
    {
        Task NotifyOrderCreatedAsync(Order order);
    }
}
