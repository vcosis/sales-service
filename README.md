# SalesService

## Overview

SalesService is a modular .NET 8 solution designed with Project Layer Separation, following Clean Architecture principles. The project is structured to ensure clear separation of concerns, maintainability, and scalability.

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

## Documentation
See the `docs/Teste .NET` folder for business rules, API contracts, and further requirements.