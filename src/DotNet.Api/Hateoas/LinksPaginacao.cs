using DotNet.Domain.Consultas;
using Microsoft.AspNetCore.WebUtilities;

namespace DotNet.Api.Hateoas;

public static class LinksPaginacao
{
    public static IReadOnlyList<LinkHateoas> Criar<T>(HttpRequest requisicao, ResultadoPaginado<T> resultado)
    {
        var caminho = $"{requisicao.PathBase}{requisicao.Path}";

        string Montar(int pagina)
        {
            IDictionary<string, string?> consulta = new Dictionary<string, string?>();

            foreach (var parametro in requisicao.Query)
            {
                if (!EhChaveDePaginacao(parametro.Key))
                {
                    consulta[parametro.Key] = parametro.Value.ToString();
                }
            }

            consulta["pagina"] = pagina.ToString();
            consulta["tamanhoPagina"] = resultado.TamanhoPagina.ToString();

            return QueryHelpers.AddQueryString(caminho, consulta);
        }

        var links = new List<LinkHateoas>
        {
            new("self", Montar(resultado.Pagina), "GET"),
            new("first", Montar(1), "GET")
        };

        if (resultado.TemAnterior)
        {
            links.Add(new("prev", Montar(Math.Min(resultado.Pagina - 1, resultado.TotalPaginas)), "GET"));
        }

        if (resultado.TemProxima)
        {
            links.Add(new("next", Montar(resultado.Pagina + 1), "GET"));
        }

        if (resultado.TotalPaginas > 0)
        {
            links.Add(new("last", Montar(resultado.TotalPaginas), "GET"));
        }

        return links;
    }

    private static bool EhChaveDePaginacao(string chave) =>
        chave.Equals("pagina", StringComparison.OrdinalIgnoreCase) ||
        chave.Equals("tamanhoPagina", StringComparison.OrdinalIgnoreCase);
}
