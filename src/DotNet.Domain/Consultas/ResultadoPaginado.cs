namespace DotNet.Domain.Consultas;

public record ResultadoPaginado<T>(
    IReadOnlyList<T> Itens,
    int Pagina,
    int TamanhoPagina,
    int TotalItens)
{
    public int TotalPaginas => TamanhoPagina == 0
        ? 0
        : (int)Math.Ceiling(TotalItens / (double)TamanhoPagina);

    public bool TemAnterior => Pagina > 1;

    public bool TemProxima => Pagina < TotalPaginas;

    public ResultadoPaginado<TSaida> Mapear<TSaida>(Func<T, TSaida> conversor) =>
        new(Itens.Select(conversor).ToList(), Pagina, TamanhoPagina, TotalItens);
}