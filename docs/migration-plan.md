# Migration Plan: .NET Framework 4.7.2 -> .NET 8

## 1. Overview

This document outlines the migration of **Contoso.Invoicing.Legacy** from .NET Framework 4.7.2 (ASP.NET Web API 2 + OWIN) to .NET 8 (ASP.NET Core). The goal is a functional port that preserves all API behavior while adopting modern .NET patterns.

## 2. Migration Strategy

**Approach:** Bottom-up, layer by layer.

1. Domain (no changes expected)
2. Application (minimal changes)
3. Infrastructure (minimal changes)
4. Api (largest change surface)
5. Tests (switch to `Microsoft.NET.Test.Sdk` / MSTest on .NET 8)

## 3. Project File Changes

| Legacy (.csproj)                       | .NET 8 (.csproj)                        |
|----------------------------------------|-----------------------------------------|
| `packages.config` + old-style csproj   | SDK-style csproj with `<PackageReference>` |
| `<TargetFrameworkVersion>v4.7.2`       | `<TargetFramework>net8.0`               |
| `<Compile Include="...">` item lists   | Implicit globbing (remove explicit includes) |
| `AssemblyInfo.cs` hand-written         | Auto-generated (or retained if customized) |

## 4. Hosting Changes

| Aspect               | Legacy (OWIN/IIS)                           | .NET 8 (Kestrel)                            |
|----------------------|---------------------------------------------|----------------------------------------------|
| Host                 | IIS via `Microsoft.Owin.Host.SystemWeb`     | Kestrel (self-hosted) or IIS out-of-process  |
| Entry point          | `[assembly: OwinStartup]` + `Startup.Configuration(IAppBuilder)` | `Program.cs` with `WebApplication.CreateBuilder()` |
| Pipeline             | `IAppBuilder.Use<TMiddleware>()`            | `app.UseMiddleware<T>()` or `app.Use()`      |

### Action Items
- Remove `Microsoft.Owin.*` and `Owin` packages.
- Create `Program.cs` with `WebApplicationBuilder` / `WebApplication`.
- Convert `Startup.Configuration()` logic to the ASP.NET Core middleware pipeline.

## 5. Middleware Changes

### API Key Middleware

| Legacy                                      | .NET 8                                        |
|---------------------------------------------|-----------------------------------------------|
| Inherits `OwinMiddleware`                   | Implements `IMiddleware` or uses inline delegate |
| Constructor receives `OwinMiddleware next`  | Constructor receives `RequestDelegate next`   |
| `IOwinContext` for request/response         | `HttpContext`                                 |
| `context.Request.Headers.Get("X-Api-Key")` | `context.Request.Headers["X-Api-Key"]`        |
| `context.Response.WriteAsync()`             | `context.Response.WriteAsync()`               |

### Action Items
- Rewrite `ApiKeyMiddleware` to use `RequestDelegate` and `HttpContext`.
- Register with `app.UseMiddleware<ApiKeyMiddleware>()`.

## 6. Routing Changes

| Legacy                                | .NET 8                                      |
|---------------------------------------|---------------------------------------------|
| `config.MapHttpAttributeRoutes()`     | `app.MapControllers()` (attribute routing)  |
| `[RoutePrefix("api/batches")]`        | `[Route("api/batches")]` on controller      |
| `[Route("{batchId:guid}/run")]`       | Same syntax (compatible)                    |
| `config.Routes.MapHttpRoute()`        | Convention routes via `app.MapControllerRoute()` or attribute routing only |

### Action Items
- Replace `[RoutePrefix]` with `[Route]` on controller class (or use `[ApiController]`).
- Add `[ApiController]` attribute for automatic model validation and `[FromBody]` inference.
- Remove `WebApiConfig.cs`; configure in `Program.cs`.

## 7. Controller Changes

| Legacy                                | .NET 8                                      |
|---------------------------------------|---------------------------------------------|
| Inherits `ApiController`              | Inherits `ControllerBase`                   |
| `IHttpActionResult`                   | `IActionResult` or `ActionResult<T>`        |
| `return Ok(...)`, `return NotFound()` | Same method names (different namespace)     |
| `return Created(uri, value)`          | `return CreatedAtAction(...)` or `Created()`|
| `Configuration.Properties["key"]`     | Constructor DI via `IServiceProvider`       |

### Action Items
- Change base class to `ControllerBase`.
- Replace `IHttpActionResult` with `ActionResult<T>`.
- Replace manual service lookup (`Configuration.Properties`) with constructor injection.
- Register services in `builder.Services` (DI container).

## 8. Dependency Injection

| Legacy                                                | .NET 8                                             |
|-------------------------------------------------------|-----------------------------------------------------|
| Manual composition root in `Startup`                  | `builder.Services.AddSingleton/Scoped/Transient<>()` |
| Services stored in `HttpConfiguration.Properties`     | Constructor injection via built-in DI               |

### Action Items
- Register `IInvoiceBatchRepository`, `IInvoiceRepository`, `IEventPublisher`, `IBatchService` in the DI container.
- Remove `WireUpDependencies()` method and property bag approach.

## 9. Configuration Changes

| Legacy                                        | .NET 8                                          |
|-----------------------------------------------|--------------------------------------------------|
| `Web.config` + `ConfigurationManager`         | `appsettings.json` + `IConfiguration`            |
| `Web.Release.config` transforms               | `appsettings.Production.json` or env variables   |
| `ConfigurationManager.AppSettings["ApiKey"]`  | `configuration["ApiKey"]` or options pattern     |

### Action Items
- Create `appsettings.json` with same keys.
- Create `appsettings.Production.json` (replaces `Web.Release.config`).
- Inject `IConfiguration` where needed (e.g., middleware).
- Remove `Web.config` and `Web.Release.config`.

## 10. Serialization Changes

| Legacy                                         | .NET 8                                          |
|------------------------------------------------|--------------------------------------------------|
| Newtonsoft.Json (default)                      | System.Text.Json (default) or opt-in Newtonsoft  |
| `config.Formatters.JsonFormatter.SerializerSettings` | `builder.Services.AddControllers().AddJsonOptions()` |

### Action Items
- Decide: migrate to `System.Text.Json` or keep Newtonsoft via `AddNewtonsoftJson()`.
- Replicate camelCase, null handling, date format settings.
- Remove XML formatter removal code (not present by default in ASP.NET Core).

## 11. Test Project Changes

| Legacy                              | .NET 8                                       |
|-------------------------------------|----------------------------------------------|
| Old-style csproj + packages.config  | SDK-style csproj                             |
| MSTest on .NET Framework            | MSTest on .NET 8 (`Microsoft.NET.Test.Sdk`)  |

### Action Items
- Convert to SDK-style csproj targeting `net8.0`.
- Add `Microsoft.NET.Test.Sdk`, `MSTest.TestAdapter`, `MSTest.TestFramework` as `PackageReference`.
- Tests should compile and pass without logic changes.

## 12. Packages to Remove

- `Microsoft.Owin` / `Microsoft.Owin.Host.SystemWeb`
- `Owin`
- `Microsoft.AspNet.WebApi.Core` / `Client` / `Owin`

## 13. Packages to Add

- `Microsoft.AspNetCore.App` (framework reference, implicit)
- Optionally: `Microsoft.AspNetCore.Mvc.NewtonsoftJson` if keeping Newtonsoft

## 14. Migration Checklist

- [ ] Convert all csproj files to SDK-style targeting `net8.0`
- [ ] Remove `packages.config` files; add `<PackageReference>` entries
- [ ] Remove `AssemblyInfo.cs` or consolidate with csproj auto-generation
- [ ] Create `Program.cs` replacing OWIN Startup
- [ ] Rewrite `ApiKeyMiddleware` for ASP.NET Core
- [ ] Update controllers (base class, return types, DI)
- [ ] Replace `Web.config` with `appsettings.json`
- [ ] Configure JSON serialization in `Program.cs`
- [ ] Register services in DI container
- [ ] Update test project references and SDK
- [ ] Verify all endpoints return identical responses
- [ ] Run all unit tests
- [ ] Update CI pipeline for `dotnet build` / `dotnet test`

## 15. Risk Areas

| Risk                                  | Mitigation                                      |
|---------------------------------------|--------------------------------------------------|
| JSON serialization differences        | Integration tests comparing response payloads    |
| Route matching behavior changes       | Side-by-side route testing                       |
| Missing assembly binding redirects    | Not needed in .NET 8; just remove                |
| Config transform logic in deployment  | Replace with environment-specific appsettings    |
