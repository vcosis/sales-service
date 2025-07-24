# SalesService

## Overview

SalesService is a modular .NET 8 solution designed with Project Layer Separation, following Clean Architecture principles. The project is structured to ensure clear separation of concerns, maintainability, and scalability.

## Quick Start with Docker

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
- **API:** http://localhost:5000
- **Swagger UI:** http://localhost:5000/swagger
- **Database:** localhost:5432 (postgres/S@le5#01!)

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
      SaleTests.cs                   # Domain entity tests (39 tests)
      SaleItemTests.cs               # Sale item business rules tests
  Application/
    Queries/
      GetSalesQueryHandlerTests.cs   # Query handler tests (1 test)
```

### Test Coverage

The test suite covers:

- **Domain Layer (39 tests)**: Business rules, entity validation, and domain logic
  - Sale entity: creation, update, cancellation, item management
  - SaleItem entity: discount calculations, quantity validations
- **Application Layer (1 test)**: Query handlers with filtering, ordering, and pagination

**Total: 40 tests** - All passing ✅

### Test Frameworks

- **xUnit**: Testing framework
- **NSubstitute**: Mocking framework
- **FluentAssertions**: Fluent assertion syntax
- **Bogus**: Fake data generation

## Documentation
See the `docs/Teste .NET` folder for business rules, API contracts, and further requirements.