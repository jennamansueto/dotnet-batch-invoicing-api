using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Contoso.Invoicing.Application.Dtos;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace Contoso.Invoicing.IntegrationTests
{
    public class ApiIntegrationTests : IClassFixture<WebApplicationFactory<Contoso.Invoicing.Api.Program>>
    {
        private readonly HttpClient _client;
        private static readonly JsonSerializerOptions JsonOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public ApiIntegrationTests(WebApplicationFactory<Contoso.Invoicing.Api.Program> factory)
        {
            _client = factory.CreateClient();
        }

        [Fact]
        public async Task HealthCheck_ReturnsOk()
        {
            var response = await _client.GetAsync("/health");
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateBatch_WithoutApiKey_ReturnsUnauthorized()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var json = JsonSerializer.Serialize(request, JsonOptions);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await _client.PostAsync("/api/batches", content);

            Assert.Equal(HttpStatusCode.Unauthorized, response.StatusCode);
        }

        [Fact]
        public async Task CreateBatch_WithApiKey_ReturnsCreated()
        {
            var request = new CreateBatchRequest
            {
                CustomerIds = new List<string> { "CUST-001" },
                FromDate = new DateTime(2023, 1, 1),
                ToDate = new DateTime(2023, 1, 31),
                RequestedBy = "admin@contoso.com"
            };

            var json = JsonSerializer.Serialize(request, JsonOptions);
            var httpRequest = new HttpRequestMessage(HttpMethod.Post, "/api/batches")
            {
                Content = new StringContent(json, Encoding.UTF8, "application/json")
            };
            httpRequest.Headers.Add("X-Api-Key", "contoso-dev-key-2023");

            var response = await _client.SendAsync(httpRequest);
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }

        [Fact]
        public async Task GetBatch_NonExistent_ReturnsNotFound()
        {
            var httpRequest = new HttpRequestMessage(HttpMethod.Get, $"/api/batches/{Guid.NewGuid()}");
            httpRequest.Headers.Add("X-Api-Key", "contoso-dev-key-2023");

            var response = await _client.SendAsync(httpRequest);
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}
