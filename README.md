# SalesService

## Overview

SalesService is a modular .NET 8 solution designed with Project Layer Separation, following Clean Architecture principles. The project is structured to ensure clear separation of concerns, maintainability, and scalability.

## Quick Start (Manual Setup)

### Prerequisites
- .NET 8.0 SDK
- PostgreSQL 15+ (or Docker for database only)

### Database Setup

#### Option 1: Using Docker for PostgreSQL only
```bash
# Start PostgreSQL container
docker run -d \
  --name sales-postgres \
  -e POSTGRES_DB=salesdb \
  -e POSTGRES_USER=postgres \
  -e POSTGRES_PASSWORD=S@le5#01! \
  -p 5432:5432 \
  postgres:15-alpine

# Wait for database to be ready (check logs)
docker logs sales-postgres
```

#### Option 2: Local PostgreSQL Installation
1. Install PostgreSQL 15+ on your system
2. Create database: `salesdb`
3. Create user: `postgres` with password: `S@le5#01!`
4. Ensure PostgreSQL is running on port 5432

### Connection String Configuration

The application uses the following connection string:
```
Host=localhost;Database=salesdb;Username=postgres;Password=S@le5#01!
```

This is configured in `src/SalesService.Api/appsettings.json` and can be overridden via environment variables.

### Running the Application

#### 1. Restore Dependencies
```bash
# Restore all packages
dotnet restore

# Or restore specific project
dotnet restore src/SalesService.Api/SalesService.Api.csproj
```

#### 2. Build the Solution
```bash
# Build all projects
dotnet build

# Build specific project
dotnet build src/SalesService.Api/SalesService.Api.csproj
```

#### 3. Run Database Migrations
```bash
# Apply migrations to create database schema
dotnet ef database update --project src/SalesService.Infrastructure --startup-project src/SalesService.Api
```

#### 4. Start the API
```bash
# Run the API project
dotnet run --project src/SalesService.Api

# Or navigate to the API directory and run
cd src/SalesService.Api
dotnet run
```

#### 5. Verify Application is Running
- **API**: http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger

### Application Startup Process

When you run the application, you should see the following logs:

```
🔄 Running database migrations...
✅ Migrations completed successfully!
✅ Database seeded successfully!
🚀 Sales Service API started successfully!
🌐 Swagger UI available at: http://localhost:5000/swagger
```

### Testing the API

Once the application is running, you can test it using:

#### 1. Swagger UI
- Open http://localhost:5000/swagger in your browser
- Explore available endpoints
- Test API calls directly from the interface

#### 2. Using curl
```bash
# Get all sales
curl -X GET "http://localhost:5000/api/sales" -H "accept: application/json"

# Get sales with pagination
curl -X GET "http://localhost:5000/api/sales?page=1&size=5" -H "accept: application/json"

# Get sales filtered by cancelled status
curl -X GET "http://localhost:5000/api/sales?filter=cancelled=true" -H "accept: application/json"

# Get specific sale by ID
curl -X GET "http://localhost:5000/api/sales/1" -H "accept: application/json"
```

#### 3. Using Postman or similar tools
- Import the Swagger JSON from http://localhost:5000/swagger/v1/swagger.json
- Test all endpoints with proper request/response validation

### Troubleshooting

#### Database Connection Issues
- Ensure PostgreSQL is running and accessible
- Verify connection string in `appsettings.json`
- Check if port 5432 is not blocked by firewall

#### Build Errors
- Ensure .NET 8.0 SDK is installed: `dotnet --version`
- Clear NuGet cache: `dotnet nuget locals all --clear`
- Delete `bin/` and `obj/` folders and rebuild

#### Migration Issues
- Ensure database exists and user has proper permissions
- Check if migrations are up to date: `dotnet ef migrations list`

#### API Not Responding
- Check if the application is running on the correct port
- Verify no other application is using port 5000
- Check application logs for startup errors

### Stopping the Application

#### Stop the API
- Press `Ctrl+C` in the terminal where the API is running
- Or kill the process: `dotnet run` will show the process ID

#### Stop PostgreSQL (if using Docker)
```bash
# Stop the container
docker stop sales-postgres

# Remove the container (data will be lost)
docker rm sales-postgres

# Or keep the container for next time
docker start sales-postgres
```

#### Clean Up (Optional)
```bash
# Remove all build artifacts
dotnet clean

# Remove database (if using Docker)
docker volume rm $(docker volume ls -q | grep postgres)
```

## Quick Start with Docker (Alternative)

There is an issue running via Docker.
In the meantime, consider running via Manual Setup.

### Prerequisites
- Docker Desktop installed and running

### Run the application
```bash
# Build and start all services (PostgreSQL + API)
docker-compose up --build -d

# Check if services are running
docker-compose ps

# View logs
docker-compose logs -f
```

### Access the application
- **Swagger UI:** http://localhost:5000/swagger


### Stop the application
```bash
docker-compose down
```

For detailed Docker instructions, see [DOCKER_README.md](DOCKER_README.md).

## Solution Structure

```
SalesService.sln
src/
  SalesService.Api/             # Presentation layer (Web API)
    SalesService.Api.csproj
  SalesService.Application/     # Application layer (use cases, CQRS, MediatR)
    SalesService.Application.csproj
  SalesService.Domain/          # Domain layer (entities, business rules, events)
    SalesService.Domain.csproj
  SalesService.Infrastructure/  # Infrastructure layer (EF Core, external services)
    SalesService.Infrastructure.csproj
```

### Project Descriptions
- **SalesService.Api**: ASP.NET Core Web API, responsible for HTTP endpoints and request handling.
- **SalesService.Application**: Application logic, use cases, CQRS, MediatR handlers, DTOs.
- **SalesService.Domain**: Core business entities, value objects, domain services, and business rules.
- **SalesService.Infrastructure**: Data persistence (EF Core), external integrations, repository implementations.

## Tech Stack
- .NET 8.0 (C#)
- MediatR, AutoMapper, EF Core
- PostgreSQL
- Domain Events (DDD pattern)
- Rebus (Message Broker - optional)

## Database Migrations

To generate the initial database migrations (without applying them), run:

```
dotnet ef migrations add InitialCreate --project src/SalesService.Infrastructure --startup-project src/SalesService.Api
```

## Testing

### Running Tests

The project includes comprehensive unit tests using xUnit, NSubstitute for mocking, and FluentAssertions for assertions.

#### Run all tests
```bash
dotnet test
```

#### Run tests with detailed output
```bash
dotnet test --verbosity normal
```

#### Run tests without building (faster for subsequent runs)
```bash
dotnet test --no-build
```

#### Run specific test project
```bash
dotnet test tests/SalesService.Tests.csproj
```

#### Run tests with coverage (requires coverlet.collector)
```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test Structure

```
tests/
  SalesService.Tests.csproj          # Test project configuration
  GlobalUsings.cs                    # Global using directives
  Domain/
    Entities/
      SaleTests.cs                   # Domain entity tests (19 tests)
      SaleItemTests.cs               # Sale item business rules tests (13 tests)
      SaleDomainEventsTests.cs       # Domain events tests (9 tests)
  Application/
    Queries/
      GetSalesQueryHandlerTests.cs   # Query handler tests (8 tests)
```

### Test Coverage

The test suite covers:

- **Domain Layer (41 tests)**: Business rules, entity validation, domain logic, and domain events
  - Sale entity: creation, update, cancellation, item management (19 tests)
  - SaleItem entity: discount calculations, quantity validations (13 tests)
  - Domain Events: event generation, collection, and clearing (9 tests)
- **Application Layer (8 tests)**: Query handlers with filtering, ordering, and pagination

**Total: 49 tests** - All passing ✅

### Test Frameworks

- **xUnit**: Testing framework
- **NSubstitute**: Mocking framework
- **FluentAssertions**: Fluent assertion syntax
- **Faker**: Fake data generation

## Domain Events

The application implements Domain-Driven Design (DDD) events pattern, allowing loose coupling between domain logic and external systems.

### Event Types

The following domain events are automatically generated by the `Sale` entity:

- **SaleCreatedEvent**: Fired when a new sale is created
- **SaleModifiedEvent**: Fired when sale properties or items are modified
- **SaleCancelledEvent**: Fired when a sale is cancelled
- **ItemCancelledEvent**: Fired when an item is removed from a sale

### How Events Work

1. **Event Generation**: Domain entities generate events internally during business operations
2. **Event Collection**: Command handlers collect events from entities after successful operations
3. **Event Publishing**: Events are published through the configured `IEventPublisher`
4. **Event Processing**: Publishers handle events according to their implementation

### Event Publisher Configuration

The application supports multiple event publishing strategies:

#### Current Setup (Rebus In-Memory)
```csharp
// In Program.cs
builder.Services.AddScoped<IEventPublisher, RebusEventPublisher>();
```

#### Switch to Message Broker (RabbitMQ)
```csharp
// 1. Add Rebus packages to .csproj files
// 2. Configure Rebus in Program.cs
// 3. Change registration to:
builder.Services.AddScoped<IEventPublisher, RebusEventPublisher>();
```

### Testing Domain Events

Run the domain events tests:
```bash
dotnet test --filter "SaleDomainEventsTests"
```

These tests verify that:
- Events are generated correctly by domain entities
- Events contain the expected data
- Event collection and clearing works properly

### Verifying Events in Action

When the application is running, you can see events being published in the console logs:

```
info: SalesService.Infrastructure.Events.LoggingEventPublisher[0]
      📢 DOMAIN EVENT PUBLISHED: SaleCreatedEvent for Sale 0 at 07/24/2025 03:07:55
info: SalesService.Infrastructure.Events.LoggingEventPublisher[0]
      🛒 Sale Created: a005 for Deviliz with 0 items, Total: ¤0.00
```

### Event Data Structure

Each event contains relevant business data:

- **SaleCreatedEvent**: SaleId, SaleNumber, CustomerName, ItemsCount, TotalAmount
- **SaleModifiedEvent**: SaleId, SaleNumber, CustomerName, TotalAmount, ModificationType
- **SaleCancelledEvent**: SaleId, SaleNumber, CustomerName, TotalAmount, CancelledAt
- **ItemCancelledEvent**: SaleId, ItemId, ProductId, ProductName, CancellationReason

### Benefits

1. **Loose Coupling**: Domain logic doesn't depend on external systems
2. **Flexibility**: Easy to switch between different event publishing strategies
3. **Testability**: Events can be tested independently
4. **Scalability**: Can easily add new event consumers without changing domain logic
5. **Audit Trail**: All domain changes are automatically tracked

## Documentation
See the `docs/Teste .NET` folder for business rules, API contracts, and further requirements.
