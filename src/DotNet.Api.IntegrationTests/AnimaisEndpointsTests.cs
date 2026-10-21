using System.Net;
using System.Net.Http.Json;
using DotNet.Application.Dtos;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

[Collection("Integration Tests")]
public class AnimaisEndpointsTests
{
    private readonly HttpClient _client;

    public AnimaisEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task FluxoCompleto_CriarResponsavelEAnimal_DeveRetornar201ComRecursoPersistido()
    {
        // Arrange
        var novoResponsavel = new
        {
            name = "Mariana Oliveira",
            email = "mariana.oliveira@email.com",
            phone = "11977776666",
            cpf = Guid.NewGuid().ToString("N")[..11]
        };

        // Act — cria o responsável
        var responsavelResponse = await _client.PostAsJsonAsync("/api/responsaveis", novoResponsavel);

        // Assert
        responsavelResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var responsavelCriado = await responsavelResponse.Content.ReadFromJsonAsync<ResponsavelResponse>();
        responsavelCriado.Should().NotBeNull();

        // Act — cria o animal vinculado ao responsável recém-criado
        var novoAnimal = new
        {
            responsavelId = responsavelCriado!.Id,
            name = "Thor",
            nickname = "Toto",
            species = "DOG",
            breed = "Pug",
            birthDate = "2020-05-10T00:00:00",
            weight = 8.5,
            sex = "MALE",
            rga = Guid.NewGuid().ToString("N")[..10]
        };

        var animalResponse = await _client.PostAsJsonAsync("/api/animais", novoAnimal);

        // Assert
        animalResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        var animalCriado = await animalResponse.Content.ReadFromJsonAsync<AnimalResponse>();
        animalCriado!.ResponsavelId.Should().Be(responsavelCriado.Id);
    }

    [Fact]
    public async Task Create_ComResponsavelIdInexistente_DeveRetornar400()
    {
        // Arrange
        var animalComResponsavelInvalido = new
        {
            responsavelId = 999999,
            name = "Thor",
            nickname = "Toto",
            species = "DOG",
            breed = "Pug",
            birthDate = "2020-05-10T00:00:00",
            weight = 8.5,
            sex = "MALE",
            rga = Guid.NewGuid().ToString("N")[..10]
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/animais", animalComResponsavelInvalido);

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
    }

    [Fact]
    public async Task GetById_ComAnimalInexistente_DeveRetornar404()
    {
        // Act
        var response = await _client.GetAsync("/api/animais/999999");

        // Assert
        response.StatusCode.Should().Be(HttpStatusCode.NotFound);
    }
}