using System.Text.Json;
using System.Text.Json.Serialization;
using Contoso.Invoicing.Api.Configuration;
using Contoso.Invoicing.Api.Middleware;
using Contoso.Invoicing.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<InvoicingOptions>(
    builder.Configuration.GetSection(InvoicingOptions.SectionName));

builder.Services.AddInvoicingServices();

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
        options.JsonSerializerOptions.WriteIndented = true;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });

builder.Services.AddHealthChecks();

var app = builder.Build();

app.UseMiddleware<ApiKeyMiddleware>();

app.MapHealthChecks("/api/health", new HealthCheckOptions
{
    ResponseWriter = (context, report) =>
    {
        context.Response.ContentType = "application/json";
        var status = report.Status == HealthStatus.Healthy ? "OK" : report.Status.ToString();
        return context.Response.WriteAsync("{\"status\":\"" + status + "\"}");
    }
});

app.MapControllers();

app.Run();

public partial class Program { }
