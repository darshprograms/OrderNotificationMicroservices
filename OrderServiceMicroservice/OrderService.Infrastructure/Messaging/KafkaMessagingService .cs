using Confluent.Kafka;
using Microsoft.Extensions.Configuration;
using OrderService.Application.Interfaces;
using System.Text.Json;

namespace OrderService.Infrastructure.Messaging
{
    public class KafkaMessagingService : IMessagingService
    {
        private readonly IProducer<string, string> _producer;
        private readonly string _topic;

        public KafkaMessagingService(IConfiguration config)
        {
            var kafkaConfig = new ProducerConfig
            {
                BootstrapServers = config["Kafka:BootstrapServers"]
            };

            _producer = new ProducerBuilder<string, string>(kafkaConfig).Build();
            _topic = "orders.created";
        }

        public Task PublishOrderCreatedAsync(Guid orderId, DateTime timestamp)
        {
            // Fire-and-forget task to prevent blocking
            _ = Task.Run(async () =>
            {
                try
                {
                    var message = new Message<string, string>
                    {
                        Key = orderId.ToString(),
                        Value = JsonSerializer.Serialize(new { orderId, timestamp })
                    };

                    var deliveryResult = await _producer.ProduceAsync(_topic, message);
                    Console.WriteLine($"📤 Kafka message sent to {deliveryResult.TopicPartitionOffset}");
                }
                catch (KafkaException ex)
                {
                    Console.WriteLine($"⚠️ Kafka is not reachable. Skipping publish. Reason: {ex.Message}");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Unexpected Kafka error: {ex.Message}");
                }
            });

            // Return immediately, don't await
            return Task.CompletedTask;
        }

    }
}
