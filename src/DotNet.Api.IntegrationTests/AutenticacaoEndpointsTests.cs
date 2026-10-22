using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using DotNet.Application.Dtos;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

[Collection("Integration Tests")]
public class AutenticacaoEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;

    public AutenticacaoEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Login_ComCredenciaisValidas_DeveRetornar200ComToken()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin-teste", "senha-teste"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var token = await response.Content.ReadFromJsonAsync<TokenResponse>();
        token!.AccessToken.Should().NotBeNullOrWhiteSpace();
        token.TokenType.Should().Be("Bearer");
    }

    [Fact]
    public async Task Login_ComSenhaIncorreta_DeveRetornar401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.PostAsJsonAsync("/api/auth/login", new LoginRequest("admin-teste", "errada"));

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAnimais_SemToken_DeveRetornar401()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Act
        var response = await client.GetAsync("/api/animais");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAnimais_ComTokenInvalido_DeveRetornar401()
    {
        // Arrange
        var client = _factory.CreateClient();
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", "token-invalido");

        // Act
        var response = await client.GetAsync("/api/animais");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    [Fact]
    public async Task GetAnimais_ComTokenValido_DeveRetornar200()
    {
        // Arrange
        var client = _factory.CriarClienteAutenticado();

        // Act
        var response = await client.GetAsync("/api/animais");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}