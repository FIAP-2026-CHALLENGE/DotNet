using System.Net;
using System.Net.Http.Json;
using System.Text.Json;
using DotNet.Application.Dtos;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

[Collection("Integration Tests")]
public class PaginacaoEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public PaginacaoEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CriarClienteAutenticado();
    }

    [Fact]
    public async Task GetAnimais_PrimeiraPagina_DeveRetornarItensTotaisELinksDeNavegacao()
    {
        // Arrange
        var responsavelId = await CriarResponsavelAsync();
        await CriarAnimalAsync(responsavelId, "Alfa");
        await CriarAnimalAsync(responsavelId, "Bidu");
        await CriarAnimalAsync(responsavelId, "Cesar");

        // Act
        var json = await ObterJsonAsync($"/api/animais?responsavelId={responsavelId}&tamanhoPagina=2");

        // Assert
        json.GetProperty("itens").GetArrayLength().Should().Be(2);
        json.GetProperty("totalItens").GetInt32().Should().Be(3);
        json.GetProperty("totalPaginas").GetInt32().Should().Be(2);

        var links = json.GetProperty("links");
        Rels(links).Should().BeEquivalentTo("self", "first", "next", "last");
        Href(links, "next").Should().Contain("pagina=2").And.Contain($"responsavelId={responsavelId}");
    }

    [Fact]
    public async Task GetAnimais_UltimaPagina_DeveTerLinkPrevESemLinkNext()
    {
        // Arrange
        var responsavelId = await CriarResponsavelAsync();
        await CriarAnimalAsync(responsavelId, "Alfa");
        await CriarAnimalAsync(responsavelId, "Bidu");
        await CriarAnimalAsync(responsavelId, "Cesar");

        // Act
        var json = await ObterJsonAsync($"/api/animais?responsavelId={responsavelId}&pagina=2&tamanhoPagina=2");

        // Assert
        json.GetProperty("itens").GetArrayLength().Should().Be(1);
        Rels(json.GetProperty("links")).Should().BeEquivalentTo("self", "first", "prev", "last");
    }

    [Fact]
    public async Task GetAnimais_OrdenadosPorNome_DeveRetornarEmOrdemCrescente()
    {
        // Arrange
        var responsavelId = await CriarResponsavelAsync();
        await CriarAnimalAsync(responsavelId, "Bidu");
        await CriarAnimalAsync(responsavelId, "Cesar");
        await CriarAnimalAsync(responsavelId, "Alfa");

        // Act
        var json = await ObterJsonAsync($"/api/animais?responsavelId={responsavelId}&ordenarPor=name");

        // Assert
        NomeDoItem(json, 0).Should().Be("Alfa");
    }

    [Fact]
    public async Task GetAnimais_OrdenadosPorNomeDecrescente_DeveRetornarEmOrdemInversa()
    {
        // Arrange
        var responsavelId = await CriarResponsavelAsync();
        await CriarAnimalAsync(responsavelId, "Bidu");
        await CriarAnimalAsync(responsavelId, "Cesar");
        await CriarAnimalAsync(responsavelId, "Alfa");

        // Act
        var json = await ObterJsonAsync($"/api/animais?responsavelId={responsavelId}&ordenarPor=name&decrescente=true");

        // Assert
        NomeDoItem(json, 0).Should().Be("Cesar");
    }

    [Fact]
    public async Task GetAnimais_ComFiltroSemResultado_DeveRetornarListaVazia()
    {
        // Arrange
        var responsavelId = await CriarResponsavelAsync();
        await CriarAnimalAsync(responsavelId, "Alfa");

        // Act
        var json = await ObterJsonAsync($"/api/animais?responsavelId={responsavelId}&species=CAT");

        // Assert
        json.GetProperty("itens").GetArrayLength().Should().Be(0);
        json.GetProperty("totalItens").GetInt32().Should().Be(0);
    }

    [Fact]
    public async Task GetAnimais_ComTamanhoDePaginaAcimaDoMaximo_DeveLimitarEm50()
    {
        // Act
        var json = await ObterJsonAsync("/api/animais?tamanhoPagina=999");

        // Assert
        json.GetProperty("tamanhoPagina").GetInt32().Should().Be(50);
    }

    [Fact]
    public async Task GetAnimalPorId_DeveRetornarDadosELinksRelacionados()
    {
        // Arrange
        var responsavelId = await CriarResponsavelAsync();
        var animalId = await CriarAnimalAsync(responsavelId, "Thor");

        // Act
        var json = await ObterJsonAsync($"/api/animais/{animalId}");

        // Assert
        json.GetProperty("dados").GetProperty("id").GetInt32().Should().Be(animalId);

        var links = json.GetProperty("links");
        Rels(links).Should().BeEquivalentTo("self", "update", "delete", "responsavel", "care-events");
        Href(links, "self").Should().Be($"/api/animais/{animalId}");
        Href(links, "responsavel").Should().Be($"/api/responsaveis/{responsavelId}");
        Href(links, "care-events").Should().Contain($"animalId={animalId}");
    }

    [Fact]
    public async Task GetAnimais_ComResponsavelIdInexistente_DeveRetornar404()
    {
        // Act
        var response = await _client.GetAsync("/api/animais?responsavelId=999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetCareEvents_ComStatusInvalido_DeveRetornar400()
    {
        // Act
        var response = await _client.GetAsync("/api/care-events?status=INEXISTENTE");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetCareEvents_SemToken_DeveRetornar401()
    {
        // Arrange
        var clienteSemToken = _factory.CreateClient();

        // Act
        var response = await clienteSemToken.GetAsync("/api/care-events");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }

    private async Task<int> CriarResponsavelAsync()
    {
        var response = await _client.PostAsJsonAsync("/api/responsaveis", new
        {
            name = "Responsavel Teste",
            email = "teste@email.com",
            phone = "11999999999",
            cpf = Guid.NewGuid().ToString("N")[..11]
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var criado = await response.Content.ReadFromJsonAsync<ResponsavelResponse>();
        return criado!.Id;
    }

    private async Task<int> CriarAnimalAsync(int responsavelId, string nome)
    {
        var response = await _client.PostAsJsonAsync("/api/animais", new
        {
            responsavelId,
            name = nome,
            nickname = "Apelido",
            species = "DOG",
            breed = "Pug",
            birthDate = "2020-05-10T00:00:00",
            weight = 8.5,
            sex = "MALE",
            rga = Guid.NewGuid().ToString("N")[..10]
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);

        var criado = await response.Content.ReadFromJsonAsync<AnimalResponse>();
        return criado!.Id;
    }

    private async Task<JsonElement> ObterJsonAsync(string url)
    {
        var response = await _client.GetAsync(url);

        response.StatusCode.Should().Be(HttpStatusCode.OK);

        return await response.Content.ReadFromJsonAsync<JsonElement>();
    }

    private static string? NomeDoItem(JsonElement json, int posicao) =>
        json.GetProperty("itens")[posicao].GetProperty("dados").GetProperty("name").GetString();

    private static string[] Rels(JsonElement links) =>
        links.EnumerateArray().Select(l => l.GetProperty("rel").GetString()!).ToArray();

    private static string Href(JsonElement links, string rel) =>
        links.EnumerateArray()
            .First(l => l.GetProperty("rel").GetString() == rel)
            .GetProperty("href").GetString()!;
}
