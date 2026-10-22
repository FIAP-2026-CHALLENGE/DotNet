using DotNet.Domain.Consultas;

namespace DotNet.Api.Hateoas;

public record ColecaoPaginada<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens,
    int TotalPaginas,
    IReadOnlyList<LinkHateoas> Links)
{
    public static ColecaoPaginada<T> De(ResultadoPaginado<T> resultado, IReadOnlyList<LinkHateoas> links) =>
        new(resultado.Itens, resultado.Pagina, resultado.TamanhoPagina, resultado.TotalItens, resultado.TotalPaginas, links);
}
