using System.Net;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

[Collection("Integration Tests")]
public class HealthEndpointTests
{
    private readonly HttpClient _client;

    public HealthEndpointTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Health_DeveRetornarOkQuandoAplicacaoEstaSaudavel()
    {
        // Act
        var response = await _client.GetAsync("/health");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}