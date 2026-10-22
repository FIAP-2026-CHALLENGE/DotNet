namespace DotNet.Domain.Excecoes;

/// <summary>Lançada quando as credenciais informadas no login são inválidas.</summary>
public class NaoAutorizadoException : Exception
{
    public NaoAutorizadoException(string mensagem) : base(mensagem)
    {
    }
}