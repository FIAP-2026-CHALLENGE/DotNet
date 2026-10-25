using System.Net;
using System.Net.Http.Json;
using DotNet.Api.Hateoas;
using DotNet.Application.Dtos;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

[Collection("Integration Tests")]
public class RelatosEndpointsTests
{
    private readonly CustomWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public RelatosEndpointsTests(CustomWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CriarClienteAutenticado();
    }

    private async Task<int> CriarAnimalAsync()
    {
        var responsavel = await _client.PostAsJsonAsync("/api/responsaveis", new
        {
            name = "Paula Souza",
            email = $"{Guid.NewGuid():N}@email.com",
            phone = "11955554444",
            cpf = Guid.NewGuid().ToString("N")[..11]
        });
        var responsavelCriado = await responsavel.Content.ReadFromJsonAsync<ResponsavelResponse>();

        var animal = await _client.PostAsJsonAsync("/api/animais", new
        {
            responsavelId = responsavelCriado!.Id,
            name = "Mel",
            nickname = "Melzinha",
            species = "CAT",
            breed = "SRD",
            birthDate = "2021-03-15T00:00:00",
            weight = 4.2,
            sex = "FEMALE",
            rga = Guid.NewGuid().ToString("N")[..10]
        });
        var animalCriado = await animal.Content.ReadFromJsonAsync<AnimalResponse>();

        return animalCriado!.Id;
    }

    [Fact]
    public async Task FluxoCompleto_CriarConsultarAtualizarERemoverRelato_DeveRespeitarOsStatusHttp()
    {
        // Arrange
        var animalId = await CriarAnimalAsync();
        var novoRelato = new
        {
            animalId,
            texto = "  Comeu pouco hoje e ficou deitado  ",
            dataOcorrencia = "2026-10-24T20:00:00Z",
            sintomas = new[] { "Apatia ", "apatia", "falta de apetite" }
        };

        // Act — cria
        var criacao = await _client.PostAsJsonAsync("/api/relatos", novoRelato);

        // Assert
        criacao.StatusCode.Should().Be(HttpStatusCode.Created);
        var criado = await criacao.Content.ReadFromJsonAsync<RelatoTutorResponse>();
        criado!.Texto.Should().Be("Comeu pouco hoje e ficou deitado");
        criado.Sintomas.Should().Equal("apatia", "falta de apetite");

        // Act — consulta com links HATEOAS
        var consulta = await _client.GetFromJsonAsync<Recurso<RelatoTutorResponse>>($"/api/relatos/{criado.Id}");

        // Assert
        consulta!.Dados.Id.Should().Be(criado.Id);
        consulta.Links.Select(l => l.Rel).Should().BeEquivalentTo("self", "update", "delete", "animal");
        consulta.Links.Single(l => l.Rel == "animal").Href.Should().Be($"/api/animais/{animalId}");

        // Act — atualiza
        var atualizacao = await _client.PutAsJsonAsync($"/api/relatos/{criado.Id}", new
        {
            texto = "Melhorou à tarde",
            dataOcorrencia = "2026-10-24T20:00:00Z",
            sintomas = new[] { "apatia" }
        });

        // Assert
        atualizacao.StatusCode.Should().Be(HttpStatusCode.NoContent);
        var atualizado = await _client.GetFromJsonAsync<Recurso<RelatoTutorResponse>>($"/api/relatos/{criado.Id}");
        atualizado!.Dados.Texto.Should().Be("Melhorou à tarde");

        // Act — remove
        var remocao = await _client.DeleteAsync($"/api/relatos/{criado.Id}");

        // Assert
        remocao.StatusCode.Should().Be(HttpStatusCode.NoContent);
        (await _client.GetAsync($"/api/relatos/{criado.Id}")).StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_ComFiltroDeSintoma_DeveRetornarSoRelatosComOSintoma()
    {
        // Arrange
        var animalId = await CriarAnimalAsync();
        await _client.PostAsJsonAsync("/api/relatos", new { animalId, texto = "Vomitou", dataOcorrencia = "2026-10-20T10:00:00Z", sintomas = new[] { "vômito" } });
        await _client.PostAsJsonAsync("/api/relatos", new { animalId, texto = "Desanimado", dataOcorrencia = "2026-10-21T10:00:00Z", sintomas = new[] { "apatia" } });

        // Act
        var resposta = await _client.GetFromJsonAsync<ColecaoPaginada<Recurso<RelatoTutorResponse>>>(
            $"/api/relatos?animalId={animalId}&sintoma=VÔMITO");

        // Assert
        resposta!.TotalItens.Should().Be(1);
        resposta.Itens.Single().Dados.Texto.Should().Be("Vomitou");
        resposta.Links.Should().Contain(l => l.Rel == "self");
    }

    [Fact]
    public async Task Create_ComAnimalInexistente_DeveRetornar400()
    {
        // Act
        var response = await _client.PostAsJsonAsync("/api/relatos", new
        {
            animalId = 999999,
            texto = "Comeu pouco",
            dataOcorrencia = "2026-10-24T20:00:00Z"
        });

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ComIdInexistente_DeveRetornar404()
    {
        // Act
        var response = await _client.GetAsync("/api/relatos/abc");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }

    [Fact]
    public async Task GetAll_SemToken_DeveRetornar401()
    {
        // Arrange
        var clienteAnonimo = _factory.CreateClient();

        // Act
        var response = await clienteAnonimo.GetAsync("/api/relatos");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
    }
}
