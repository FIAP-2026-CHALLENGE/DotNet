namespace DotNet.Domain.Excecoes;

/// <summary>
/// Lançada quando uma regra de negócio é violada (ex: CPF duplicado, RGA duplicado,
/// referência a um ResponsavelId/AnimalId inexistente). O GlobalExceptionMiddleware converte esta exceção em 404/400
/// </summary>
public class RegraDeNegocioException : Exception
{
    public RegraDeNegocioException(string mensagem) : base(mensagem)
    {
    }
}