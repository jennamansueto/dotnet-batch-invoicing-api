# Contoso.Invoicing.Legacy - Architecture

## Overview

The Finance Batch Invoicing API allows authorized callers to create invoice batches for a set of customers over a date range, run the batch generation, and retrieve results. It is deployed as an ASP.NET Web API 2 application hosted via OWIN on IIS (.NET Framework 4.7.2).

## Solution Structure

```
Contoso.Invoicing.Legacy.sln
├── src/
│   ├── Invoicing.Api            ASP.NET Web API 2 + OWIN host
│   ├── Invoicing.Application    Application services, DTOs
│   ├── Invoicing.Domain         Domain models, repository interfaces, event contracts
│   └── Invoicing.Infrastructure In-memory repositories, event publisher stub
├── tests/
│   └── Invoicing.UnitTests      MSTest unit tests
├── docs/                        Architecture & migration documentation
└── build/                       CI pipeline placeholder
```

## Layer Responsibilities

### Invoicing.Domain
- Pure domain models: `InvoiceBatch`, `Invoice`, `Money`, `BatchStatus`
- Repository interfaces: `IInvoiceBatchRepository`, `IInvoiceRepository`
- Event contract: `IEventPublisher`
- No external dependencies

### Invoicing.Application
- `IBatchService` / `BatchService` — orchestrates batch creation, execution, retrieval
- DTO classes (`CreateBatchRequest`, `BatchSummaryDto`, `InvoiceDto`)
- References Domain only

### Invoicing.Infrastructure
- `InMemoryInvoiceBatchRepository` / `InMemoryInvoiceRepository` — thread-safe in-memory stores using `ConcurrentDictionary` / `ConcurrentBag`
- `ConsoleEventPublisher` — logs events via `System.Diagnostics.Trace`
- References Domain only

### Invoicing.Api
- OWIN `Startup` class with manual composition root (no DI container)
- `ApiKeyMiddleware` — checks `X-Api-Key` header against `Web.config` AppSettings
- `BatchesController` — CRUD-style routes under `/api/batches`
- `HealthController` — `/api/health` (bypasses auth)
- `WebApiConfig` — attribute routing, camelCase JSON via Newtonsoft.Json
- `Web.config` / `Web.Release.config` — AppSettings + config transforms

## API Routes

| Method | Route                          | Description                    | Auth |
|--------|--------------------------------|--------------------------------|------|
| GET    | /api/health                    | Health check                   | No   |
| POST   | /api/batches                   | Create a new invoice batch     | Yes  |
| POST   | /api/batches/{batchId}/run     | Run batch generation (sync)    | Yes  |
| GET    | /api/batches/{batchId}         | Get batch status & summary     | Yes  |
| GET    | /api/batches/{batchId}/invoices| List invoices for a batch      | Yes  |

## Authentication

Simple API key validation via OWIN middleware. The expected key is stored in `Web.config`:

```xml
<add key="ApiKey" value="contoso-dev-key-2023" />
```

Clients must pass it as: `X-Api-Key: contoso-dev-key-2023`

## Serialization

- Newtonsoft.Json 13.x
- camelCase property names
- Nulls omitted
- ISO 8601 dates
- XML formatter removed

## Hosting

- IIS / IIS Express via `Microsoft.Owin.Host.SystemWeb`
- OWIN startup auto-detected via `[assembly: OwinStartup]`

## Dependencies (NuGet)

| Package                          | Version |
|----------------------------------|---------|
| Microsoft.AspNet.WebApi.Core     | 5.2.9   |
| Microsoft.AspNet.WebApi.Client   | 5.2.9   |
| Microsoft.AspNet.WebApi.Owin     | 5.2.9   |
| Microsoft.Owin                   | 4.2.2   |
| Microsoft.Owin.Host.SystemWeb    | 4.2.2   |
| Newtonsoft.Json                   | 13.0.3  |
| Owin                             | 1.0     |
