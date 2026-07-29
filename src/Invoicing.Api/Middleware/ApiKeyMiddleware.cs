using System;
using System.Net;
using System.Threading.Tasks;
using Contoso.Invoicing.Api.Configuration;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Options;

namespace Contoso.Invoicing.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private const string ApiKeyHeader = "X-Api-Key";
        private const string HealthPath = "/api/health";

        private readonly RequestDelegate _next;
        private readonly IOptions<InvoicingOptions> _options;

        public ApiKeyMiddleware(RequestDelegate next, IOptions<InvoicingOptions> options)
        {
            _next = next ?? throw new ArgumentNullException(nameof(next));
            _options = options ?? throw new ArgumentNullException(nameof(options));
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (path.TrimEnd('/').Equals(HealthPath, StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var providedKey = context.Request.Headers[ApiKeyHeader].ToString();
            var expectedKey = _options.Value.ApiKey;

            if (string.IsNullOrWhiteSpace(providedKey) || providedKey != expectedKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Missing or invalid API key.\"}");
                return;
            }

            await _next(context);
        }
    }
}
