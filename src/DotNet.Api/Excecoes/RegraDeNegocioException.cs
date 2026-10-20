namespace DotNet.Api.Excecoes;

/// <summary>
/// Lançada quando uma regra de negócio é violada (ex: CPF duplicado, RGA duplicado,
/// referência a um ResponsavelId/AnimalId inexistente). Os Controllers capturam esta
/// exceção e retornam 400 Bad Request.
/// </summary>
public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem) : base(mensagem)
    {
    }
}