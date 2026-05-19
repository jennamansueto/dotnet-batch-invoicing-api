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
        private readonly RequestDelegate _next;

        public ApiKeyMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context, IOptions<InvoicingOptions> options)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (path.TrimEnd('/').Equals("/health", StringComparison.OrdinalIgnoreCase))
            {
                await _next(context);
                return;
            }

            var providedKey = context.Request.Headers[ApiKeyHeader].ToString();
            var expectedKey = options.Value.ApiKey;

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
