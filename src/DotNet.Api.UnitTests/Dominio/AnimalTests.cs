using DotNet.Api.Models;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.UnitTests.Dominio;

public class AnimalTests
{
    private static readonly DateTime BirthDate = new(2020, 5, 10);

    [Theory]
    [InlineData("DOG")]
    [InlineData("CAT")]
    [InlineData("dog")]
    public void Criar_ComEspecieValida_DeveNormalizarSpeciesParaMaiusculo(string species)
    {
        // Arrange & Act
        var animal = Animal.Criar(1, "Thor", "Toto", species, "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Assert
        animal.Species.Should().Be(species.ToUpper());
        animal.IsActive.Should().BeTrue();
    }

    [Theory]
    [InlineData("BIRD")]
    [InlineData("HORSE")]
    [InlineData("")]
    public void Criar_ComEspecieNaoPermitida_DeveLancarArgumentException(string species)
    {
        // Arrange
        Action act = () => Animal.Criar(1, "Thor", "Toto", species, "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(-10.5)]
    public void Criar_ComPesoZeradoOuNegativo_DeveLancarArgumentException(decimal weight)
    {
        // Arrange
        Action act = () => Animal.Criar(1, "Thor", "Toto", "DOG", "Pug", BirthDate, weight, "MALE", "RGA123456");

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Criar_ComResponsavelIdInvalido_DeveLancarArgumentException()
    {
        // Arrange
        Action act = () => Animal.Criar(0, "Thor", "Toto", "DOG", "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Act & Assert
        act.Should().Throw<ArgumentException>();
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAlterarPropriedades()
    {
        // Arrange
        var animal = Animal.Criar(1, "Thor", "Toto", "DOG", "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Act
        animal.Atualizar(1, "Thor", "Thorzinho", "DOG", "Pug", BirthDate, 9.2m, "MALE", "RGA123456", true);

        // Assert
        animal.Nickname.Should().Be("Thorzinho");
        animal.Weight.Should().Be(9.2m);
    }
}