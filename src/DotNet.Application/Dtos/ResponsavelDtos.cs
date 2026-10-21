using DotNet.Domain.Entidades;

namespace DotNet.Application.Dtos;

public record CriarResponsavelRequest(string Name, string Email, string Phone, string Cpf);

public record AtualizarResponsavelRequest(
    string Name, string Email, string Phone, string Cpf, bool IsActive = true);

public record ResponsavelResponse(
    int Id, string Name, string Email, string Phone, string Cpf, DateTime CreatedAt, bool IsActive)
{
    public static ResponsavelResponse De(Responsavel r) =>
        new(r.Id, r.Name, r.Email, r.Phone, r.Cpf, r.CreatedAt, r.IsActive);
}
