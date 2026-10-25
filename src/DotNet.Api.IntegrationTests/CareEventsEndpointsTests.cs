using System.Net;
using System.Net.Http.Json;
using DotNet.Api.Hateoas;
using DotNet.Application.Dtos;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.IntegrationTests;

[Collection("Integration Tests")]
public class CareEventsEndpointsTests
{
    private readonly HttpClient _client;

    public CareEventsEndpointsTests(CustomWebApplicationFactory factory)
    {
        _client = factory.CriarClienteAutenticado();
    }

    private async Task<CareEventResponse> CriarEventoAsync(string status)
    {
        var animalId = await CenariosDeTeste.CriarAnimalAsync(_client);

        var response = await _client.PostAsJsonAsync("/api/care-events", new
        {
            petId = animalId,
            type = "CHECKUP",
            title = "Check-up anual",
            scheduledDate = "2026-11-10T10:00:00",
            status,
            priority = "HIGH"
        });

        response.StatusCode.Should().Be(HttpStatusCode.Created);
        return (await response.Content.ReadFromJsonAsync<CareEventResponse>())!;
    }

    [Fact]
    public async Task GetById_ComEventoPendente_DeveOferecerLinkComplete()
    {
        // Arrange
        var evento = await CriarEventoAsync("PENDING");

        // Act
        var recurso = await _client.GetFromJsonAsync<Recurso<CareEventResponse>>($"/api/care-events/{evento.Id}");

        // Assert
        recurso!.Links.Should().Contain(l => l.Rel == "complete" && l.Method == "PATCH");
    }

    [Fact]
    public async Task GetById_DepoisDeConcluido_NaoDeveOferecerLinkComplete()
    {
        // Arrange
        var evento = await CriarEventoAsync("PENDING");
        (await _client.PatchAsync($"/api/care-events/{evento.Id}/complete", null)).StatusCode.Should().Be(HttpStatusCode.NoContent);

        // Act
        var recurso = await _client.GetFromJsonAsync<Recurso<CareEventResponse>>($"/api/care-events/{evento.Id}");

        // Assert
        recurso!.Dados.Status.Should().Be("COMPLETED");
        recurso.Links.Should().NotContain(l => l.Rel == "complete");
        recurso.Links.Select(l => l.Rel).Should().Contain(["self", "update", "delete", "animal"]);
    }
}
