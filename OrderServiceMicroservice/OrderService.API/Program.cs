using MediatR;
using OrderService.Application.Interfaces;
using OrderService.Infrastructure.Messaging;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;
using StackExchange.Redis;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// 🔧 Configure Services
// ==============================

// ✅ This is the correct syntax for v11
builder.Services.AddMediatR(typeof(OrderService.Application.Commands.CreateOrder.CreateOrderCommand).GetTypeInfo().Assembly);

// Register API controllers
builder.Services.AddControllers();

// Swagger/OpenAPI support (.NET 9 style)
builder.Services.AddOpenApi();

builder.Services.AddScoped<IOrderRepository, InMemoryOrderRepository>();

// Redis configuration
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = builder.Configuration.GetValue<string>("Redis:ConnectionString");

    var options = ConfigurationOptions.Parse(configuration);
    options.AbortOnConnectFail = false;

    return ConnectionMultiplexer.Connect(options);
});

// ✅ Conditional registration with SSL bypass for dev only
if (builder.Environment.IsDevelopment())
{
    builder.Services.AddHttpClient<INotificationService, NotificationServiceClient>(client =>
    {
        client.BaseAddress = new Uri("http://localhost:5070"); // Match your app launch URL
    })
    .ConfigurePrimaryHttpMessageHandler(() =>
    {
        return new HttpClientHandler
        {
            // ⚠️ Development only — skip certificate validation
            ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
        };
    });
}
else
{
    // 🔐 In production, use secure validation and real domain
    builder.Services.AddHttpClient<INotificationService, NotificationServiceClient>(client =>
    {
        client.BaseAddress = new Uri("https://production-url-or-service-discovery"); // Replace in real env
    });
}

builder.Services.AddSingleton<IMessagingService, KafkaMessagingService>();

var app = builder.Build();

// ==============================
// 🚀 Configure Middleware
// ==============================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Enables /swagger and /openapi.json
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
