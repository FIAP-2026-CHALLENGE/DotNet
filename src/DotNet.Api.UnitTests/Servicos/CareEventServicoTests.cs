using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Api.Servicos;
using FluentAssertions;
using Moq;
using Xunit;

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
        var careEvent = await _servico.CriarAsync(
            1, "CHECKUP", "Check-up", "Descrição", ScheduledDate, null, "PENDING", "HIGH", null);

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
        Func<Task> act = async () => await _servico.CriarAsync(
            99, "CHECKUP", "Check-up", "Descrição", ScheduledDate, null, "PENDING", "HIGH", null);

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
    public async Task ListarPorStatusAsync_ComStatusInvalido_DeveLancarRegraDeNegocioException()
    {
        // Act
        Func<Task> act = async () => await _servico.ListarPorStatusAsync("INEXISTENTE");

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
    }
}