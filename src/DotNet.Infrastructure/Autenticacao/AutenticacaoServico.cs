using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using DotNet.Application.Dtos;
using DotNet.Application.Servicos;
using DotNet.Domain.Excecoes;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;

namespace DotNet.Infrastructure.Autenticacao;

public class AutenticacaoServico : IAutenticacaoServico
{
    private readonly JwtOpcoes _jwt;
    private readonly CredenciaisAdminOpcoes _admin;

    public AutenticacaoServico(IOptions<JwtOpcoes> jwt, IOptions<CredenciaisAdminOpcoes> admin)
    {
        _jwt = jwt.Value;
        _admin = admin.Value;
    }

    public TokenResponse Autenticar(LoginRequest request)
    {
        if (!CredenciaisValidas(request))
        {
            throw new NaoAutorizadoException("Usuário ou senha inválidos.");
        }

        var expiraEm = DateTime.UtcNow.AddMinutes(_jwt.ExpiraEmMinutos);
        var chave = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwt.Key));
        var assinatura = new SigningCredentials(chave, SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, request.Usuario),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var token = new JwtSecurityToken(
            issuer: _jwt.Issuer,
            audience: _jwt.Audience,
            claims: claims,
            expires: expiraEm,
            signingCredentials: assinatura);

        return new TokenResponse(new JwtSecurityTokenHandler().WriteToken(token), "Bearer", expiraEm);
    }

    private bool CredenciaisValidas(LoginRequest request)
    {
        if (string.IsNullOrEmpty(_admin.Usuario) || string.IsNullOrEmpty(_admin.Senha))
        {
            return false;
        }

        // '&' (e não '&&') avalia os dois lados sempre, sem revelar qual campo errou pelo tempo de resposta.
        return IguaisEmTempoConstante(request.Usuario, _admin.Usuario)
             & IguaisEmTempoConstante(request.Senha, _admin.Senha);
    }

    private static bool IguaisEmTempoConstante(string a, string b) =>
        CryptographicOperations.FixedTimeEquals(
            SHA256.HashData(Encoding.UTF8.GetBytes(a)),
            SHA256.HashData(Encoding.UTF8.GetBytes(b)));
}
