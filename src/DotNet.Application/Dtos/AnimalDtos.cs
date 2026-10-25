using DotNet.Domain.Entidades;

namespace DotNet.Application.Dtos;

public record CriarAnimalRequest(
    int ResponsavelId, string Name, string Species, string Breed,
    DateTime BirthDate, decimal Weight, string Sex,
    string Nickname = "", string Rga = "");

public record AtualizarAnimalRequest(
    int ResponsavelId, string Name, string Species, string Breed,
    DateTime BirthDate, decimal Weight, string Sex,
    string Nickname = "", string Rga = "", bool IsActive = true);

public record AnimalResponse(
    int Id, int ResponsavelId, string Name, string Nickname, string Species, string Breed,
    DateTime BirthDate, decimal Weight, string Sex, string Rga, DateTime CreatedAt, bool IsActive)
{
    public static AnimalResponse De(Animal a) =>
        new(a.Id, a.ResponsavelId, a.Name, a.Nickname, a.Species, a.Breed,
            a.BirthDate, a.Weight, a.Sex, a.Rga, a.CreatedAt, a.IsActive);
}
