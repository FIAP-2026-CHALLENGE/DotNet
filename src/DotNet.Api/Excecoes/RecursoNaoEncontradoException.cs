namespace DotNet.Api.Excecoes;

/// <summary>
/// Lançada quando uma entidade solicitada (Responsavel, Animal ou CareEvent) não existe.
/// Os Controllers capturam esta exceção e retornam 404 Not Found.
/// </summary>
public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}