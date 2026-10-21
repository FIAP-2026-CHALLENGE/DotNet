using System.IdentityModel.Tokens.Jwt;
using DotNet.Application.Dtos;
using DotNet.Domain.Excecoes;
using DotNet.Infrastructure.Autenticacao;
using FluentAssertions;
using Microsoft.Extensions.Options;
using Xunit;

namespace DotNet.Api.UnitTests.Autenticacao;

public class AutenticacaoServicoTests
{
    private static AutenticacaoServico CriarServico(string usuario = "admin", string senha = "senha-forte")
    {
        var jwt = Options.Create(new JwtOpcoes
        {
            Key = "chave-de-teste-com-mais-de-trinta-e-dois-caracteres",
            Issuer = "ClyvoVet.Api",
            Audience = "ClyvoVet.Clientes",
            ExpiraEmMinutos = 30
        });
        var admin = Options.Create(new CredenciaisAdminOpcoes { Usuario = usuario, Senha = senha });

        return new AutenticacaoServico(jwt, admin);
    }

    [Fact]
    public void Autenticar_ComCredenciaisValidas_DeveGerarTokenComIssuerAudienceEUsuario()
    {
        // Arrange
        var servico = CriarServico();

        // Act
        var resposta = servico.Autenticar(new LoginRequest("admin", "senha-forte"));

        // Assert
        var token = new JwtSecurityTokenHandler().ReadJwtToken(resposta.AccessToken);
        token.Issuer.Should().Be("ClyvoVet.Api");
        token.Audiences.Should().Contain("ClyvoVet.Clientes");
        token.Subject.Should().Be("admin");
        resposta.TokenType.Should().Be("Bearer");
        resposta.ExpiraEm.Should().BeAfter(DateTime.UtcNow);
    }

    [Theory]
    [InlineData("admin", "senha-errada")]
    [InlineData("outro", "senha-forte")]
    [InlineData("outro", "senha-errada")]
    public void Autenticar_ComCredenciaisInvalidas_DeveLancarNaoAutorizadoException(string usuario, string senha)
    {
        // Arrange
        var servico = CriarServico();

        // Act
        Action act = () => servico.Autenticar(new LoginRequest(usuario, senha));

        // Assert
        act.Should().Throw<NaoAutorizadoException>();
    }

    [Fact]
    public void Autenticar_ComCredenciaisAdminNaoConfiguradas_DeveLancarNaoAutorizadoException()
    {
        // Arrange
        var servico = CriarServico(usuario: "", senha: "");

        // Act
        Action act = () => servico.Autenticar(new LoginRequest("", ""));

        // Assert
        act.Should().Throw<NaoAutorizadoException>();
    }
}