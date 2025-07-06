using OrderService.Application.Interfaces;
using OrderService.Domain.Entities;
using Polly;
using Polly.Retry;
using System.Net.Http.Json;

namespace OrderService.Infrastructure.Services
{
    public class NotificationServiceClient : INotificationService
    {
        private readonly HttpClient _httpClient;
        private readonly AsyncRetryPolicy _retryPolicy;

        public NotificationServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;

            _retryPolicy = Policy
                .Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, attempt => TimeSpan.FromSeconds(Math.Pow(2, attempt)));
        }

        public async Task NotifyOrderCreatedAsync(Order order)
        {
            await _retryPolicy.ExecuteAsync(() =>
                _httpClient.PostAsJsonAsync("/notification", order));
        }
    }
}
