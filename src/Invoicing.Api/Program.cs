using System.Text.Json;
using System.Text.Json.Serialization;
using Contoso.Invoicing.Api.Configuration;
using Contoso.Invoicing.Api.Middleware;
using Contoso.Invoicing.Infrastructure.DependencyInjection;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;

namespace Contoso.Invoicing.Api
{
    public partial class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.Configure<InvoicingOptions>(
                builder.Configuration.GetSection("Invoicing"));

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
                    options.JsonSerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
                    options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
                    options.JsonSerializerOptions.WriteIndented = true;
                });

            builder.Services.AddHealthChecks();
            builder.Services.AddInvoicingServices();

            var app = builder.Build();

            app.UseMiddleware<CorrelationIdMiddleware>();
            app.UseMiddleware<ExceptionHandlingMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();

            app.MapControllers();
            app.MapHealthChecks("/health");

            app.Run();
        }
    }
}
