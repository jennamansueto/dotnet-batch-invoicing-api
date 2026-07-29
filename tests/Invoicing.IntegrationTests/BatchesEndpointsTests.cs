using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Contoso.Invoicing.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Contoso.Invoicing.IntegrationTests
{
    public class BatchesEndpointsTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private const string ApiKeyHeader = "X-Api-Key";
        private const string ApiKey = "contoso-dev-key-2023";

        private readonly WebApplicationFactory<Program> _factory;

        public BatchesEndpointsTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory;
        }

        private HttpClient CreateAuthenticatedClient()
        {
            var client = _factory.CreateClient();
            client.DefaultRequestHeaders.Add(ApiKeyHeader, ApiKey);
            return client;
        }

        private static CreateBatchRequest SampleRequest()
        {
            return new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001", "CUST-002" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };
        }

        [Fact]
        public async Task Health_ReturnsOk_WithoutApiKey()
        {
            var client = _factory.CreateClient();

            var response = await client.GetAsync("/api/health");
            var body = await response.Content.ReadAsStringAsync();

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.Contains("\"status\":\"OK\"", body);
        }

        [Fact]
        public async Task Batches_WithoutApiKey_ReturnsUnauthorized()
        {
            var client = _factory.CreateClient();

            var response = await client.PostAsJsonAsync("/api/batches", SampleRequest());

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateRunAndListInvoices_RoundTrips()
        {
            var client = CreateAuthenticatedClient();

            var createResponse = await client.PostAsJsonAsync("/api/batches", SampleRequest());
            Assert.Equal(HttpStatusCode.Created, createResponse.StatusCode);

            var created = await createResponse.Content.ReadFromJsonAsync<BatchSummaryDto>();
            Assert.Equal("Pending", created.Status);

            var runResponse = await client.PostAsync($"/api/batches/{created.BatchId}/run", null);
            Assert.Equal(HttpStatusCode.OK, runResponse.StatusCode);

            var ran = await runResponse.Content.ReadFromJsonAsync<BatchSummaryDto>();
            Assert.Equal("Completed", ran.Status);
            Assert.Equal(2, ran.InvoiceCount);

            var getResponse = await client.GetAsync($"/api/batches/{created.BatchId}");
            Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);

            var invoicesResponse = await client.GetAsync($"/api/batches/{created.BatchId}/invoices");
            Assert.Equal(HttpStatusCode.OK, invoicesResponse.StatusCode);

            var invoices = await invoicesResponse.Content.ReadFromJsonAsync<List<InvoiceDto>>();
            Assert.Equal(2, invoices.Count);
        }

        [Fact]
        public async Task GetBatch_Unknown_ReturnsNotFound()
        {
            var client = CreateAuthenticatedClient();

            var response = await client.GetAsync($"/api/batches/{Guid.NewGuid()}");

            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task CreateBatch_ResponseUsesCamelCaseJson()
        {
            var client = CreateAuthenticatedClient();

            var response = await client.PostAsJsonAsync("/api/batches", SampleRequest());
            var body = await response.Content.ReadAsStringAsync();

            Assert.Contains("\"batchId\"", body);
            Assert.Contains("\"requestedBy\"", body);
            Assert.DoesNotContain("\"BatchId\"", body);
            Assert.DoesNotContain("failureReason", body);
        }
    }
}
