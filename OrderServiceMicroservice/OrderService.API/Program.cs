using MediatR;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// ==============================
// ?? Configure Services
// ==============================

// ? This is the correct syntax for v11
builder.Services.AddMediatR(typeof(OrderService.Application.Commands.CreateOrder.CreateOrderCommand).GetTypeInfo().Assembly);

// Register API controllers
builder.Services.AddControllers();

// Swagger/OpenAPI support (.NET 9 style)
builder.Services.AddOpenApi();

var app = builder.Build();

// ==============================
// ?? Configure Middleware
// ==============================

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi(); // Enables /swagger and /openapi.json
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
