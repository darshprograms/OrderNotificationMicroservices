# OrderServiceMicroservice

Microservice to handle customer orders in a distributed e-commerce system.

---

## ✨ Overview

This service implements a RESTful **OrderService** with the following capabilities:

* **POST /orders**: Create a new order
* **GET /orders/{id}**: Retrieve order details
* Caches responses using **Redis** (5 min expiration)
* Notifies a **NotificationService** (HTTP POST)
* Publishes events to **Kafka** (`orders.created` topic)
* Follows **Clean Architecture** and **CQRS (MediatR)** pattern

---

## 🎓 Architecture Decisions

| Concern         | Technology Used       | Rationale                                                              |
| --------------- | --------------------- | ---------------------------------------------------------------------- |
| API Framework   | ASP.NET Core (.NET 9) | Lightweight and modern for REST APIs                                   |
| CQRS + Mediator | MediatR               | Separates business logic from API, cleanly implements commands/queries |
| Caching         | StackExchange.Redis   | Fast response for `GET /orders/{id}`                                   |
| Messaging       | Confluent.Kafka       | Publishes `orders.created` events asynchronously                       |
| Retry Policy    | Polly                 | Adds fault-tolerance for HTTP call to NotificationService              |
| HTTP Client     | HttpClientFactory     | For NotificationService client with retry and SSL bypass in dev        |

---

## 🧳 Integration Points

### 1. Notification Service (HTTP)

* Sends order data to `/notification` endpoint
* Uses `HttpClient` with Polly retry policy
* Skips SSL verification in dev for local testing

### 2. Kafka

* Topic: `orders.created`
* Produces message with `orderId` and `timestamp`
* Wrapped in `try-catch` to avoid blocking API if Kafka fails

### 3. Redis Cache

* Endpoint: `GET /orders/{id}` caches order for 5 minutes
* Configurable connection string: `Redis:ConnectionString`

---

## 💼 How to Run Locally

### Prerequisites

* [.NET 9 SDK](https://dotnet.microsoft.com/en-us/download)
* Redis running locally (`localhost:6379`)
* NotificationService mocked as local controller (`/notification`)
* Optional: Kafka (if unavailable, Kafka publish fails gracefully)

### Redis Setup (Dev)

```bash
# Run Redis via Docker
docker run -d -p 6379:6379 redis
```

### Run the Service

```bash
dotnet build
cd OrderService.API
dotnet run
```

API will be hosted at: `http://localhost:5070`

### Test with Postman

* **Create Order:**

  * `POST /orders`
  * JSON body:

```json
{
  "customerId": 101,
  "items": [
    { "productId": 1, "quantity": 2 },
    { "productId": 3, "quantity": 5 }
  ]
}
```

* **Get Order:**

  * `GET /orders/{id}`

---

## 🛎️ Assumptions Made

* Product catalog/validation is out of scope
* Notification service is internal (no auth)
* Redis/Kafka are external dependencies (optionally Dockerized)
* Notification retries are not persisted (fire-and-forget)

---

## 🚨 Troubleshooting

| Issue                        | Resolution                                                            |
| ---------------------------- | --------------------------------------------------------------------- |
| Redis not connected          | Ensure Redis is running on `localhost:6379`                           |
| Kafka timeout / stuck        | Kafka not running – message is skipped; no retry or dead letter queue |
| SSL certificate error in dev | Disabled using `DangerousAcceptAnyServerCertificateValidator`         |

---

## 📚 Production Readiness Notes

| Aspect        | Recommendations                                                               |
| ------------- | ----------------------------------------------------------------------------- |
| Redis         | Use clustered Redis with proper TTL and connection resiliency                 |
| Kafka         | Set up with producer retries, delivery reports, and monitoring                |
| Retry Policy  | Move retry logic to background worker for durability                          |
| Config        | Move secrets (e.g., Redis/Kafka connection) to Azure Key Vault or AWS Secrets |
| Logging       | Use Serilog or centralized logging (e.g., ELK, CloudWatch, Azure Monitor)     |
| Security      | Add AuthN/AuthZ using JWT or API Key-based validation                         |
| Deployment    | Use Docker containers with `docker-compose` or deploy to Kubernetes           |
| Observability | Integrate OpenTelemetry, Prometheus, Grafana                                  |

---

## 📊 NFR Highlights

* **Performance**: Caching optimizes repeated `GET` requests
* **Reliability**: Polly retries and fail-safe Kafka publishing
* **Scalability**: Microservice design with clear separation
* **Maintainability**: Clean Architecture and MediatR

---

## ✅ Covered Code Quality

* CQRS separation with MediatR
* Dependency injection used throughout
* Handlers and interfaces properly separated
* Retry and error handling for external calls
* `KafkaMessagingService` wrapped in try-catch for resilience

---

## 📄 Testing Approaches

* Tested:

  * `CreateOrderHandler`
  * `NotificationServiceClient`
* Can add:

  * Integration tests with in-memory hosting
  * Unit tests using Moq for Redis/Kafka/Notification

---

## 📂 Project Structure

```
OrderServiceMicroservice/
 ├── OrderService.API/                # Web API project
 ├── OrderService.Application/        # Commands, Queries, Interfaces
 ├── OrderService.Domain/             # Entities, Enums
 └── OrderService.Infrastructure/      # Repositories, Redis, Kafka, Notification
```

---

## 📆 Future Improvements

* Add pagination to `GET /orders`
* Introduce OrderStatus workflow and updates
* Add event sourcing for full audit/history
* Replace InMemoryRepo with DB (EF Core or Dapper)
* Retry Kafka with fallback logic or Outbox pattern
