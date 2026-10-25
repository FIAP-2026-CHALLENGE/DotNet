using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DotNet.Api.Aplicacao;
using DotNet.Api.Hateoas;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

[Collection("Integration Tests")]
public class InfoEndpointsTests
{
    private readonly HttpClient _client;

    public InfoEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task Get_SemToken_DeveRetornarNomeVersaoELinks()
    {
        // Act
        var response = await _client.GetAsync("/");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.OK);

        var raiz = await response.Content.ReadFromJsonAsync<Recurso<InformacoesApiResponse>>();
        raiz!.Dados.Nome.Should().Be(InformacoesApi.Nome);
        raiz.Dados.Versao.Should().Be(InformacoesApi.Versao);
        raiz.Links.Should().Contain(l => l.Rel == "relatos" && l.Href == "/api/relatos");
        raiz.Links.Should().Contain(l => l.Rel == "login" && l.Method == "POST");
    }

    [Fact]
    public async Task Swagger_DeveExibirNomeEVersaoDaApi()
    {
        // Act
        var documento = await _client.GetFromJsonAsync<JsonElement>("/swagger/v1/swagger.json");

        // Assert
        var info = documento.GetProperty("info");
        info.GetProperty("title").GetString().Should().Be(InformacoesApi.Nome);
        info.GetProperty("version").GetString().Should().Be(InformacoesApi.Versao);
    }
}
