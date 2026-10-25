using DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.UnitTests.Dominio;

public class RelatoTutorTests
{
    private static readonly DateTime DataOcorrencia = new(2026, 10, 24, 20, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void Criar_ComDadosValidos_DeveNormalizarTextoESintomas()
    {
        // Arrange & Act
        var relato = RelatoTutor.Criar(
            1, "  Comeu pouco hoje  ", ["Apatia ", "apatia", "", " Falta de apetite"], DataOcorrencia);

        // Assert
        relato.AnimalId.Should().Be(1);
        relato.Texto.Should().Be("Comeu pouco hoje");
        relato.Sintomas.Should().Equal("apatia", "falta de apetite");
        relato.Id.Should().BeEmpty();
    }

    [Fact]
    public void Criar_SemSintomas_DeveGuardarListaVazia()
    {
        // Arrange & Act
        var relato = RelatoTutor.Criar(1, "Comeu pouco hoje", null, DataOcorrencia);

        // Assert
        relato.Sintomas.Should().BeEmpty();
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Criar_ComTextoVazio_DeveLancarRegraDeNegocioException(string texto)
    {
        // Act
        var act = () => RelatoTutor.Criar(1, texto, null, DataOcorrencia);

        // Assert
        act.Should().Throw<RegraDeNegocioException>().WithMessage("*texto*obrigatório*");
    }

    [Fact]
    public void Criar_ComTextoAcimaDoLimite_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var texto = new string('a', RelatoTutor.TamanhoMaximoTexto + 1);

        // Act
        var act = () => RelatoTutor.Criar(1, texto, null, DataOcorrencia);

        // Assert
        act.Should().Throw<RegraDeNegocioException>();
    }

    [Fact]
    public void Criar_ComAnimalIdInvalido_DeveLancarRegraDeNegocioException()
    {
        // Act
        var act = () => RelatoTutor.Criar(0, "Comeu pouco hoje", null, DataOcorrencia);

        // Assert
        act.Should().Throw<RegraDeNegocioException>();
    }

    [Fact]
    public void Criar_ComDataNoFuturo_DeveLancarRegraDeNegocioException()
    {
        // Act
        var act = () => RelatoTutor.Criar(1, "Comeu pouco hoje", null, DateTime.UtcNow.AddDays(2));

        // Assert
        act.Should().Throw<RegraDeNegocioException>().WithMessage("*futuro*");
    }

    [Fact]
    public void Atualizar_ComDadosValidos_DeveAlterarTextoESintomasEManterAnimal()
    {
        // Arrange
        var relato = RelatoTutor.Criar(1, "Comeu pouco hoje", ["apatia"], DataOcorrencia);

        // Act
        relato.Atualizar("Melhorou à tarde", ["Vômito"], DataOcorrencia);

        // Assert
        relato.Texto.Should().Be("Melhorou à tarde");
        relato.Sintomas.Should().Equal("vômito");
        relato.AnimalId.Should().Be(1);
    }

    [Fact]
    public void Atualizar_ComTextoVazio_DeveLancarRegraDeNegocioExceptionENaoAlterar()
    {
        // Arrange
        var relato = RelatoTutor.Criar(1, "Comeu pouco hoje", null, DataOcorrencia);

        // Act
        var act = () => relato.Atualizar(" ", null, DataOcorrencia);

        // Assert
        act.Should().Throw<RegraDeNegocioException>();
        relato.Texto.Should().Be("Comeu pouco hoje");
    }
}
