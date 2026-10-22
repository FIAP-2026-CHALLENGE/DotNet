namespace DotNet.Domain.Consultas;

public record ParametrosPaginacao(
    int Pagina = 1,
    int TamanhoPagina = 10,
    string? OrdenarPor = null,
    bool Decrescente = false)
{
    public const int TamanhoMaximo = 50;

    public int PaginaSegura => Math.Max(Pagina, 1);

    public int TamanhoSeguro => Math.Clamp(TamanhoPagina, 1, TamanhoMaximo);

    public int Pular => (PaginaSegura - 1) * TamanhoSeguro;
}