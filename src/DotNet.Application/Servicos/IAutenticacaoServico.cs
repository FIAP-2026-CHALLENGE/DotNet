using DotNet.Application.Dtos;

namespace DotNet.Application.Servicos;

public interface IAutenticacaoServico
{
    TokenResponse Autenticar(LoginRequest request);
}