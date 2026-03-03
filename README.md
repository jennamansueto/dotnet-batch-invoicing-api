# Contoso.Invoicing.Legacy

A .NET Framework 4.7.2 ASP.NET Web API 2 application for batch invoice generation. This is the **pre-migration** codebase; see `docs/migration-plan.md` for the planned move to .NET 8.

## Solution Structure

```
src/
  Invoicing.Api              Web API 2 + OWIN host
  Invoicing.Application      Application services & DTOs
  Invoicing.Domain           Domain models & interfaces
  Invoicing.Infrastructure   In-memory repositories, event publisher
tests/
  Invoicing.UnitTests        MSTest unit tests
docs/
  architecture.md            System architecture
  migration-plan.md          .NET 8 migration plan
build/
  azure-pipelines.yml        CI pipeline placeholder
```

## Prerequisites

- Visual Studio 2019+ or MSBuild 15+
- .NET Framework 4.7.2 Developer Pack
- NuGet CLI

## Build

```bash
nuget restore Contoso.Invoicing.Legacy.sln
msbuild Contoso.Invoicing.Legacy.sln /p:Configuration=Release
```

## Run

Open in Visual Studio and press F5, or deploy to IIS / IIS Express. The API starts on the configured port.

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

The key is configured in `src/Invoicing.Api/Web.config` under `AppSettings/ApiKey`.

## Tests

Run via Visual Studio Test Explorer or:

```bash
vstest.console tests\Invoicing.UnitTests\bin\Release\Contoso.Invoicing.UnitTests.dll
```
