using DotNet.Api.Models;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.Tests.Dominio;

public class ResponsavelTests
{
    [Fact]
    public void Criar_ComDadosValidos_DeveCriarResponsavelAtivo()
    {
        // Arrange
        var name = "Mariana Oliveira";
        var email = "mariana.oliveira@email.com";
        var phone = "11977776666";
        var cpf = "45678912300";

        // Act
        var responsavel = Responsavel.Criar(name, email, phone, cpf);

        // Assert
        responsavel.Name.Should().Be(name);
        responsavel.Email.Should().Be(email);
        responsavel.Phone.Should().Be(phone);
        responsavel.Cpf.Should().Be(cpf);
        responsavel.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("", "mariana@email.com", "11999999999", "12345678900")]
    [InlineData("Mariana", "", "11999999999", "12345678900")]
    [InlineData("Mariana", "mariana@email.com", "", "12345678900")]
    [InlineData("Mariana", "mariana@email.com", "11999999999", "")]
    public void Criar_ComDadosInvalidos_DeveLancarArgumentException(string name, string email, string phone, string cpf)
    {
        // Arrange
        Action act = () => Responsavel.Criar(name, email, phone, cpf);

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAlterarPropriedades()
    {
        // Arrange
        var responsavel = Responsavel.Criar("Mariana", "mariana@email.com", "11999999999", "12345678900");

        // Act
        responsavel.Atualizar("Mariana Oliveira", "mariana.oliveira@email.com", "11988887777", "12345678900", false);

        // Assert
        responsavel.Name.Should().Be("Mariana Oliveira");
        responsavel.Email.Should().Be("mariana.oliveira@email.com");
        responsavel.Phone.Should().Be("11988887777");
        responsavel.IsActive.Should().BeFalse();
    }
}