using DotNet.Application.Dtos;
using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Application.Servicos;
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
        var responsavel = await _servico.CriarAsync(new CriarResponsavelRequest("Mariana", "mariana@email.com", "11999999999", "12345678900"));

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
            await _servico.CriarAsync(new CriarResponsavelRequest("Mariana", "mariana@email.com", "11999999999", "12345678900"));

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

    private static Responsavel CriarResponsavel() =>
        Responsavel.Criar("Mariana", "mariana@email.com", "11999999999", "12345678900");

    [Fact]
    public async Task ObterPorIdAsync_ComIdExistente_DeveRetornarResponse()
    {
        // Arrange
        _repositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(CriarResponsavel());

        // Act
        var responsavel = await _servico.ObterPorIdAsync(1);

        // Assert
        responsavel.Name.Should().Be("Mariana");
    }

    [Fact]
    public async Task ObterPorCpfAsync_ComCpfExistente_DeveRetornarResponse()
    {
        // Arrange
        _repositorioMock.Setup(r => r.ObterPorCpfAsync("12345678900")).ReturnsAsync(CriarResponsavel());

        // Act
        var responsavel = await _servico.ObterPorCpfAsync("12345678900");

        // Assert
        responsavel.Cpf.Should().Be("12345678900");
    }

    [Fact]
    public async Task ObterPorCpfAsync_ComCpfInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _repositorioMock.Setup(r => r.ObterPorCpfAsync("00000000000")).ReturnsAsync((Responsavel?)null);

        // Act
        Func<Task> act = async () => await _servico.ObterPorCpfAsync("00000000000");

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task AtualizarAsync_ComDadosValidos_DeveAlterarResponsavelESalvar()
    {
        // Arrange
        var responsavel = CriarResponsavel();
        _repositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(responsavel);
        _repositorioMock.Setup(r => r.CpfExisteAsync("12345678900", 1)).ReturnsAsync(false);

        // Act
        await _servico.AtualizarAsync(1, new AtualizarResponsavelRequest("Mariana Souza", "mariana@email.com", "11999999999", "12345678900"));

        // Assert
        responsavel.Name.Should().Be("Mariana Souza");
        _repositorioMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComIdInexistente_DeveLancarRecursoNaoEncontradoExceptionENaoDeveSalvar()
    {
        // Arrange
        _repositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Responsavel?)null);

        // Act
        Func<Task> act = async () =>
            await _servico.AtualizarAsync(99, new AtualizarResponsavelRequest("Mariana", "mariana@email.com", "11999999999", "12345678900"));

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _repositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComCpfDeOutroResponsavel_DeveLancarRegraDeNegocioExceptionENaoDeveSalvar()
    {
        // Arrange
        _repositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(CriarResponsavel());
        _repositorioMock.Setup(r => r.CpfExisteAsync("98765432100", 1)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () =>
            await _servico.AtualizarAsync(1, new AtualizarResponsavelRequest("Mariana", "mariana@email.com", "11999999999", "98765432100"));

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _repositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }
}