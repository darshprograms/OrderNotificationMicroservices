using MediatR;
using OrderService.Application.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace OrderService.Application.Queries.GetOrder
{
    public class GetOrderHandler : IRequestHandler<GetOrderQuery, Domain.Entities.Order?>
    {
        private readonly IOrderRepository _repository;
        private readonly IDatabase _cache;

        public GetOrderHandler(IOrderRepository repository, IConnectionMultiplexer redis)
        {
            _repository = repository;
            _cache = redis.GetDatabase();
        }

        public async Task<Domain.Entities.Order?> Handle(GetOrderQuery request, CancellationToken cancellationToken)
        {
            var cacheKey = $"order:{request.OrderId}";
            Domain.Entities.Order? order = null;

            try
            {
                var cachedValue = await _cache.StringGetAsync(cacheKey);
                if (cachedValue.HasValue)
                {
                    return JsonSerializer.Deserialize<Domain.Entities.Order>(cachedValue!);
                }
            }
            catch (RedisConnectionException)
            {
                // Redis not available — continue without cache
                Console.WriteLine("⚠️ Redis not available — skipping cache.");
            }

            order = await _repository.GetOrderByIdAsync(request.OrderId);
            if (order != null)
            {
                try
                {
                    var json = JsonSerializer.Serialize(order);
                    await _cache.StringSetAsync(cacheKey, json, TimeSpan.FromMinutes(5));
                }
                catch (RedisConnectionException)
                {
                    // Skip if Redis still not available
                    Console.WriteLine("⚠️ Redis not available — failed to cache.");
                }
            }

            return order;
        }

    }
}
