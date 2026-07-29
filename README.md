# Contoso.Invoicing.Legacy

A .NET 8 (ASP.NET Core) application for batch invoice generation, migrated from .NET Framework 4.7.2 / ASP.NET Web API 2 + OWIN. See `docs/migration-plan.md` for the migration plan that was followed.

## Solution Structure

```
src/
  Invoicing.Api              ASP.NET Core host (Kestrel)
  Invoicing.Application      Application services & DTOs
  Invoicing.Domain           Domain models & interfaces
  Invoicing.Infrastructure   In-memory repositories, event publisher, DI registration
tests/
  Invoicing.UnitTests        xUnit unit tests
  Invoicing.IntegrationTests xUnit endpoint tests (WebApplicationFactory)
docs/
  architecture.md            System architecture
  migration-plan.md          .NET 8 migration plan
build/
  azure-pipelines.yml        CI pipeline
```

## Prerequisites

- .NET 8 SDK

## Build

```bash
dotnet restore Contoso.Invoicing.Legacy.sln
dotnet build Contoso.Invoicing.Legacy.sln --configuration Release
```

## Run

```bash
dotnet run --project src/Invoicing.Api
```

The API listens on `http://localhost:5080` by default (see `src/Invoicing.Api/Properties/launchSettings.json`).

## API Endpoints

| Method | Route                          | Description                  |
|--------|--------------------------------|------------------------------|
| GET    | /api/health                    | Health check (no auth)       |
| POST   | /api/batches                   | Create invoice batch         |
| POST   | /api/batches/{id}/run          | Run batch generation         |
| GET    | /api/batches/{id}              | Get batch status & summary   |
| GET    | /api/batches/{id}/invoices     | List invoices for batch      |

## Authentication

Pass the API key header on all requests except `/api/health`:

```
X-Api-Key: contoso-dev-key-2023
```

The key is configured in `src/Invoicing.Api/appsettings.json` under `Invoicing:ApiKey` (overridable per environment via `appsettings.{Environment}.json` or the `Invoicing__ApiKey` environment variable).

## Tests

```bash
dotnet test Contoso.Invoicing.Legacy.sln --configuration Release
```
