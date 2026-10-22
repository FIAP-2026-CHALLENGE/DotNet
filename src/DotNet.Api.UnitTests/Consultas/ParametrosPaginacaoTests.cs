using DotNet.Domain.Consultas;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.UnitTests.Consultas;

public class ParametrosPaginacaoTests
{
    [Theory]
    [InlineData(0, 1)]
    [InlineData(-5, 1)]
    [InlineData(3, 3)]
    public void PaginaSegura_NuncaDeveSerMenorQueUm(int pagina, int esperado)
    {
        // Arrange
        var parametros = new ParametrosPaginacao(Pagina: pagina);

        // Act
        var resultado = parametros.PaginaSegura();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Theory]
    [InlineData(0, 1)]
    [InlineData(10, 10)]
    [InlineData(999, 50)]
    public void TamanhoSeguro_DeveFicarEntreUmEOMaximoPermitido(int tamanho, int esperado)
    {
        // Arrange
        var parametros = new ParametrosPaginacao(TamanhoPagina: tamanho);

        // Act
        var resultado = parametros.TamanhoSeguro();

        // Assert
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void Pular_DeveIgnorarOsItensDasPaginasAnteriores()
    {
        // Arrange
        var parametros = new ParametrosPaginacao(Pagina: 3, TamanhoPagina: 10);

        // Act
        var resultado = parametros.Pular();

        // Assert
        resultado.Should().Be(20);
    }
}
