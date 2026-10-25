using DotNet.Domain.Entidades;

namespace DotNet.Application.Dtos;

public record CriarRelatoTutorRequest(
    int AnimalId, string Texto, DateTime DataOcorrencia,
    List<string>? Sintomas = null);

public record AtualizarRelatoTutorRequest(
    string Texto, DateTime DataOcorrencia,
    List<string>? Sintomas = null);

public record RelatoTutorResponse(
    string Id, int AnimalId, string Texto, IReadOnlyList<string> Sintomas,
    DateTime DataOcorrencia, DateTime CriadoEm)
{
    public static RelatoTutorResponse De(RelatoTutor r) =>
        new(r.Id, r.AnimalId, r.Texto, r.Sintomas, r.DataOcorrencia, r.CriadoEm);
}
