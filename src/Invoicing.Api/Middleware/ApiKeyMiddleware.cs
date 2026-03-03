using System.Configuration;
using System.Net;
using System.Threading.Tasks;
using Microsoft.Owin;

namespace Contoso.Invoicing.Api.Middleware
{
    public class ApiKeyMiddleware : OwinMiddleware
    {
        private const string ApiKeyHeader = "X-Api-Key";

        public ApiKeyMiddleware(OwinMiddleware next) : base(next) { }

        public override async Task Invoke(IOwinContext context)
        {
            var path = context.Request.Path.Value ?? string.Empty;

            if (path.TrimEnd('/').Equals("/api/health", System.StringComparison.OrdinalIgnoreCase))
            {
                await Next.Invoke(context);
                return;
            }

            var providedKey = context.Request.Headers.Get(ApiKeyHeader);
            var expectedKey = ConfigurationManager.AppSettings["ApiKey"];

            if (string.IsNullOrWhiteSpace(providedKey) || providedKey != expectedKey)
            {
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"error\":\"Missing or invalid API key.\"}");
                return;
            }

            await Next.Invoke(context);
        }
    }
}
