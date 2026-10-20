using DotNet.Api.Excecoes;
using DotNet.Api.Models;
using DotNet.Api.Repositorios;
using DotNet.Api.Servicos;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotNet.Api.UnitTests.Servicos;

public class ResponsavelServicoTests
{
    private readonly Mock<IResponsavelRepositorio> _repositorioMock;
    private readonly ResponsavelServico _servico;

    public ResponsavelServicoTests()
    {
        _repositorioMock = new Mock<IResponsavelRepositorio>();
        _servico = new ResponsavelServico(_repositorioMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ComCpfNaoDuplicado_DeveAdicionarNoRepositorio()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.CpfExisteAsync("12345678900", null))
            .ReturnsAsync(false);

        // Act
        var responsavel = await _servico.CriarAsync("Mariana", "mariana@email.com", "11999999999", "12345678900");

        // Assert
        responsavel.Cpf.Should().Be("12345678900");
        _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Responsavel>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComCpfDuplicado_DeveLancarRegraDeNegocioExceptionENaoDeveAdicionar()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.CpfExisteAsync("12345678900", null))
            .ReturnsAsync(true);

        // Act
        Func<Task> act = async () =>
            await _servico.CriarAsync("Mariana", "mariana@email.com", "11999999999", "12345678900");

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _repositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Responsavel>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComIdInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Responsavel?)null);

        // Act
        Func<Task> act = async () => await _servico.ObterPorIdAsync(999);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task RemoverAsync_ComIdExistente_DeveChamarRemoverNoRepositorioUmaVez()
    {
        // Arrange
        var responsavel = Responsavel.Criar("Mariana", "mariana@email.com", "11999999999", "12345678900");

        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(1))
            .ReturnsAsync(responsavel);

        // Act
        await _servico.RemoverAsync(1);

        // Assert
        _repositorioMock.Verify(r => r.RemoverAsync(responsavel), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_ComIdInexistente_DeveLancarRecursoNaoEncontradoExceptionENaoDeveRemover()
    {
        // Arrange
        _repositorioMock
            .Setup(r => r.ObterPorIdAsync(It.IsAny<int>()))
            .ReturnsAsync((Responsavel?)null);

        // Act
        Func<Task> act = async () => await _servico.RemoverAsync(999);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _repositorioMock.Verify(r => r.RemoverAsync(It.IsAny<Responsavel>()), Times.Never);
    }
}