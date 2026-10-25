namespace DotNet.Domain.Consultas;

public record FiltroAnimal(
    string? Species = null,
    string? Breed = null,
    int? ResponsavelId = null,
    bool? Ativo = null);

public record FiltroCareEvent(
    int? AnimalId = null,
    string? Status = null,
    string? Type = null,
    bool? Atrasados = null);

public record FiltroResponsavel(
    string? Nome = null,
    bool? Ativo = null);

public record FiltroRelatoTutor(
    int? AnimalId = null,
    string? Sintoma = null);