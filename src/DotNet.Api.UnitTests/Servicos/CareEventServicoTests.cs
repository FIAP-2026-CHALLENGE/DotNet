using DotNet.Application.Dtos;
using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Application.Servicos;
using FluentAssertions;
using Moq;
using Xunit;
using DotNet.Domain.Consultas;

namespace DotNet.Api.UnitTests.Servicos;

public class CareEventServicoTests
{
    private readonly Mock<ICareEventRepositorio> _careEventRepositorioMock;
    private readonly Mock<IAnimalRepositorio> _animalRepositorioMock;
    private readonly CareEventServico _servico;

    private static readonly DateTime ScheduledDate = new(2026, 6, 10, 10, 0, 0);

    public CareEventServicoTests()
    {
        _careEventRepositorioMock = new Mock<ICareEventRepositorio>();
        _animalRepositorioMock = new Mock<IAnimalRepositorio>();
        _servico = new CareEventServico(_careEventRepositorioMock.Object, _animalRepositorioMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ComAnimalExistente_DeveAdicionarNoRepositorio()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);

        // Act
        var careEvent = await _servico.CriarAsync(new CriarCareEventRequest(1, "CHECKUP", "Check-up", ScheduledDate, "PENDING", "HIGH", "Descrição"));

        // Assert
        careEvent.Type.Should().Be("CHECKUP");
        _careEventRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<CareEvent>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComAnimalInexistente_DeveLancarRegraDeNegocioExceptionENaoDeveAdicionar()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _servico.CriarAsync(new CriarCareEventRequest(99, "CHECKUP", "Check-up", ScheduledDate, "PENDING", "HIGH", "Descrição"));

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _careEventRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<CareEvent>()), Times.Never);
    }

    [Fact]
    public async Task ConcluirAsync_ComEventoPendente_DeveChamarSalvarAsyncUmaVez()
    {
        // Arrange
        var careEvent = CareEvent.Criar(
            1, "CHECKUP", "Check-up", "Descrição", ScheduledDate, null, "PENDING", "HIGH", null);

        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(careEvent);

        // Act
        await _servico.ConcluirAsync(1);

        // Assert
        careEvent.Status.Should().Be("COMPLETED");
        _careEventRepositorioMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    [Fact]
    public async Task ConcluirAsync_ComEventoCancelado_DeveLancarRegraDeNegocioExceptionENaoDeveSalvar()
    {
        // Arrange
        var careEvent = CareEvent.Criar(
            1, "CHECKUP", "Check-up", "Descrição", ScheduledDate, null, "CANCELED", "HIGH", null);

        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(careEvent);

        // Act
        Func<Task> act = async () => await _servico.ConcluirAsync(1);

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _careEventRepositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    [Fact]
    public async Task ListarAsync_ComStatusInvalido_DeveLancarRegraDeNegocioException()
    {
        // Act
        Func<Task> act = async () => await _servico.ListarAsync(new FiltroCareEvent(Status: "INEXISTENTE"), new ParametrosPaginacao());

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
    }

    private static CareEvent CriarEventoPendente() =>
        CareEvent.Criar(1, "CHECKUP", "Check-up", "Descrição", ScheduledDate, null, "PENDING", "HIGH", null);

    private static AtualizarCareEventRequest RequestDeAtualizacao(int petId = 1) =>
        new(petId, "VACCINE", "Vacina anual", ScheduledDate, "PENDING", "MEDIUM", "Dose anual");

    [Fact]
    public async Task ObterPorIdAsync_ComEventoExistente_DeveRetornarResponse()
    {
        // Arrange
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(CriarEventoPendente());

        // Act
        var careEvent = await _servico.ObterPorIdAsync(1);

        // Assert
        careEvent.Title.Should().Be("Check-up");
    }

    [Fact]
    public async Task ObterPorIdAsync_ComEventoInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((CareEvent?)null);

        // Act
        Func<Task> act = async () => await _servico.ObterPorIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task AtualizarAsync_ComDadosValidos_DeveAlterarEventoESalvar()
    {
        // Arrange
        var careEvent = CriarEventoPendente();
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(careEvent);
        _animalRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);

        // Act
        await _servico.AtualizarAsync(1, RequestDeAtualizacao());

        // Assert
        careEvent.Type.Should().Be("VACCINE");
        _careEventRepositorioMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComEventoInexistente_DeveLancarRecursoNaoEncontradoExceptionENaoDeveSalvar()
    {
        // Arrange
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((CareEvent?)null);

        // Act
        Func<Task> act = async () => await _servico.AtualizarAsync(99, RequestDeAtualizacao());

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _careEventRepositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComAnimalInexistente_DeveLancarRegraDeNegocioExceptionENaoDeveSalvar()
    {
        // Arrange
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(CriarEventoPendente());
        _animalRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _servico.AtualizarAsync(1, RequestDeAtualizacao(petId: 99));

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _careEventRepositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    [Fact]
    public async Task ConcluirAsync_ComEventoInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((CareEvent?)null);

        // Act
        Func<Task> act = async () => await _servico.ConcluirAsync(99);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task RemoverAsync_ComEventoExistente_DeveChamarRemoverNoRepositorioUmaVez()
    {
        // Arrange
        var careEvent = CriarEventoPendente();
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(careEvent);

        // Act
        await _servico.RemoverAsync(1);

        // Assert
        _careEventRepositorioMock.Verify(r => r.RemoverAsync(careEvent), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_ComEventoInexistente_DeveLancarRecursoNaoEncontradoExceptionENaoDeveRemover()
    {
        // Arrange
        _careEventRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((CareEvent?)null);

        // Act
        Func<Task> act = async () => await _servico.RemoverAsync(99);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _careEventRepositorioMock.Verify(r => r.RemoverAsync(It.IsAny<CareEvent>()), Times.Never);
    }
}