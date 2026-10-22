using DotNet.Domain.Consultas;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.UnitTests.Consultas;

public class ResultadoPaginadoTests
{
    [Fact]
    public void TotalPaginas_DeveArredondarParaCima()
    {
        // Arrange
        var resultado = new ResultadoPaginado<int>(new List<int>(), 1, 10, 25);

        // Act
        var total = resultado.TotalPaginas;

        // Assert
        total.Should().Be(3);
    }

    [Theory]
    [InlineData(1, false, true)]
    [InlineData(2, true, true)]
    [InlineData(3, true, false)]
    public void TemAnteriorETemProxima_DevemDependerDaPaginaAtual(int pagina, bool temAnterior, bool temProxima)
    {
        // Arrange
        var resultado = new ResultadoPaginado<int>(new List<int>(), pagina, 10, 25);

        // Act & Assert
        resultado.TemAnterior.Should().Be(temAnterior);
        resultado.TemProxima.Should().Be(temProxima);
    }

    [Fact]
    public void Mapear_DeveConverterOsItensEManterOsNumerosDePaginacao()
    {
        // Arrange
        var resultado = new ResultadoPaginado<int>(new List<int> { 1, 2, 3 }, 2, 3, 9);

        // Act
        var mapeado = resultado.Mapear(numero => numero.ToString());

        // Assert
        mapeado.Itens.Should().Equal("1", "2", "3");
        mapeado.Pagina.Should().Be(2);
        mapeado.TamanhoPagina.Should().Be(3);
        mapeado.TotalItens.Should().Be(9);
    }
}
