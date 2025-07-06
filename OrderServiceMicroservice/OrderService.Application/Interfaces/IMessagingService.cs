namespace OrderService.Application.Interfaces
{
    public interface IMessagingService
    {
        Task PublishOrderCreatedAsync(Guid orderId, DateTime timestamp);
    }
}
