using System.Net.Http.Json;
using DotNet.Application.Dtos;

namespace DotNet.Api.IntegrationTests;

/// <summary>
/// Dados de apoio criados pela própria API, compartilhados entre as classes de teste.
/// CPF, e-mail e RGA são aleatórios para os testes não colidirem no banco InMemory compartilhado.
/// </summary>
public static class CenariosDeTeste
{
    public static async Task<int> CriarAnimalAsync(HttpClient client)
    {
        var responsavel = await client.PostAsJsonAsync("/api/responsaveis", new
        {
            name = "Paula Souza",
            email = $"{Guid.NewGuid():N}@email.com",
            phone = "11955554444",
            cpf = Guid.NewGuid().ToString("N")[..11]
        });
        var responsavelCriado = await responsavel.Content.ReadFromJsonAsync<ResponsavelResponse>();

        var animal = await client.PostAsJsonAsync("/api/animais", new
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
}
