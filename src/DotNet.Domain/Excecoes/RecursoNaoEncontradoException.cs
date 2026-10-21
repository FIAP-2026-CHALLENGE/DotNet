namespace DotNet.Domain.Excecoes;

/// <summary>
/// Lançada quando uma entidade solicitada (Responsavel, Animal ou CareEvent) não existe.
/// O GlobalExceptionMiddleware converte esta exceção em 404/400
/// </summary>
public class RecursoNaoEncontradoException : Exception
{
    public RecursoNaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}