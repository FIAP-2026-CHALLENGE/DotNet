using DotNet.Domain.Excecoes;
using DotNet.Domain.Entidades;
using DotNet.Domain.Repositorios;
using DotNet.Api.Servicos;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotNet.Api.UnitTests.Servicos;

public class AnimalServicoTests
{
    private readonly Mock<IAnimalRepositorio> _animalRepositorioMock;
    private readonly Mock<IResponsavelRepositorio> _responsavelRepositorioMock;
    private readonly AnimalServico _servico;

    private static readonly DateTime BirthDate = new(2020, 5, 10);

    public AnimalServicoTests()
    {
        _animalRepositorioMock = new Mock<IAnimalRepositorio>();
        _responsavelRepositorioMock = new Mock<IResponsavelRepositorio>();
        _servico = new AnimalServico(_animalRepositorioMock.Object, _responsavelRepositorioMock.Object);
    }

    [Fact]
    public async Task CriarAsync_ComResponsavelExistenteERgaDisponivel_DeveAdicionarNoRepositorio()
    {
        // Arrange
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _animalRepositorioMock.Setup(r => r.RgaExisteAsync("RGA123456", null)).ReturnsAsync(false);

        // Act
        var animal = await _servico.CriarAsync(1, "Thor", "Toto", "DOG", "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Assert
        animal.Name.Should().Be("Thor");
        _animalRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Animal>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComResponsavelInexistente_DeveLancarRegraDeNegocioExceptionENaoDeveAdicionar()
    {
        // Arrange
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () =>
            await _servico.CriarAsync(99, "Thor", "Toto", "DOG", "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _animalRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ComRgaJaRegistrado_DeveLancarRegraDeNegocioExceptionENaoDeveAdicionar()
    {
        // Arrange
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _animalRepositorioMock.Setup(r => r.RgaExisteAsync("RGA123456", null)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () =>
            await _servico.CriarAsync(1, "Thor", "Toto", "DOG", "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _animalRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ComEspecieInvalida_DeveLancarArgumentExceptionENaoDeveAdicionar()
    {
        // Arrange
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _animalRepositorioMock.Setup(r => r.RgaExisteAsync("RGA123456", null)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () =>
            await _servico.CriarAsync(1, "Thor", "Toto", "BIRD", "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

        // Assert
        await act.Should().ThrowAsync<ArgumentException>();
        _animalRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public async Task ListarPorResponsavelIdAsync_ComResponsavelInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _servico.ListarPorResponsavelIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }
}