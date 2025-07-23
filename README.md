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

## Documentation
See the `docs/Teste .NET` folder for business rules, API contracts, and further requirements.