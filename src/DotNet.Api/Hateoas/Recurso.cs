namespace DotNet.Api.Hateoas;

public record Recurso<T>(T Dados, IReadOnlyList<LinkHateoas> Links);
