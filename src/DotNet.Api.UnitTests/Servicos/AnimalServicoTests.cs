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
        var animal = await _servico.CriarAsync(new CriarAnimalRequest(1, "Thor", "DOG", "Pug", BirthDate, 8.5m, "MALE", "Toto", "RGA123456"));

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
            await _servico.CriarAsync(new CriarAnimalRequest(99, "Thor", "DOG", "Pug", BirthDate, 8.5m, "MALE", "Toto", "RGA123456"));

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
            await _servico.CriarAsync(new CriarAnimalRequest(1, "Thor", "DOG", "Pug", BirthDate, 8.5m, "MALE", "Toto", "RGA123456"));

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _animalRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public async Task CriarAsync_ComEspecieInvalida_DeveLancarRegraDeNegocioExceptionENaoDeveAdicionar()
    {
        // Arrange
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _animalRepositorioMock.Setup(r => r.RgaExisteAsync("RGA123456", null)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () =>
            await _servico.CriarAsync(new CriarAnimalRequest(1, "Thor", "BIRD", "Pug", BirthDate, 8.5m, "MALE", "Toto", "RGA123456"));

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _animalRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<Animal>()), Times.Never);
    }

    [Fact]
    public async Task ListarAsync_ComResponsavelInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _servico.ListarAsync(new FiltroAnimal(ResponsavelId: 99), new ParametrosPaginacao());

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    private static Animal CriarAnimal() =>
        Animal.Criar(1, "Thor", "Toto", "DOG", "Pug", BirthDate, 8.5m, "MALE", "RGA123456");

    private static AtualizarAnimalRequest RequestDeAtualizacao(int responsavelId = 1) =>
        new(responsavelId, "Thor", "DOG", "Pug", BirthDate, 9.1m, "MALE", "Toto", "RGA123456");

    [Fact]
    public async Task ObterPorIdAsync_ComAnimalExistente_DeveRetornarResponse()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(CriarAnimal());

        // Act
        var animal = await _servico.ObterPorIdAsync(1);

        // Assert
        animal.Name.Should().Be("Thor");
    }

    [Fact]
    public async Task ObterPorIdAsync_ComAnimalInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Animal?)null);

        // Act
        Func<Task> act = async () => await _servico.ObterPorIdAsync(99);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task ObterPorRgaAsync_ComRgaExistente_DeveRetornarResponse()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorRgaAsync("RGA123456")).ReturnsAsync(CriarAnimal());

        // Act
        var animal = await _servico.ObterPorRgaAsync("RGA123456");

        // Assert
        animal.Rga.Should().Be("RGA123456");
    }

    [Fact]
    public async Task ObterPorRgaAsync_ComRgaInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorRgaAsync("RGA000")).ReturnsAsync((Animal?)null);

        // Act
        Func<Task> act = async () => await _servico.ObterPorRgaAsync("RGA000");

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task AtualizarAsync_ComDadosValidos_DeveAlterarAnimalESalvar()
    {
        // Arrange
        var animal = CriarAnimal();
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(animal);
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _animalRepositorioMock.Setup(r => r.RgaExisteAsync("RGA123456", 1)).ReturnsAsync(false);

        // Act
        await _servico.AtualizarAsync(1, RequestDeAtualizacao());

        // Assert
        animal.Weight.Should().Be(9.1m);
        _animalRepositorioMock.Verify(r => r.SalvarAsync(), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComAnimalInexistente_DeveLancarRecursoNaoEncontradoExceptionENaoDeveSalvar()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Animal?)null);

        // Act
        Func<Task> act = async () => await _servico.AtualizarAsync(99, RequestDeAtualizacao());

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _animalRepositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComResponsavelInexistente_DeveLancarRegraDeNegocioExceptionENaoDeveSalvar()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(CriarAnimal());
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _servico.AtualizarAsync(1, RequestDeAtualizacao(responsavelId: 99));

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _animalRepositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComRgaDeOutroAnimal_DeveLancarRegraDeNegocioExceptionENaoDeveSalvar()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(CriarAnimal());
        _responsavelRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);
        _animalRepositorioMock.Setup(r => r.RgaExisteAsync("RGA123456", 1)).ReturnsAsync(true);

        // Act
        Func<Task> act = async () => await _servico.AtualizarAsync(1, RequestDeAtualizacao());

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _animalRepositorioMock.Verify(r => r.SalvarAsync(), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_ComAnimalExistente_DeveChamarRemoverNoRepositorioUmaVez()
    {
        // Arrange
        var animal = CriarAnimal();
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(1)).ReturnsAsync(animal);

        // Act
        await _servico.RemoverAsync(1);

        // Assert
        _animalRepositorioMock.Verify(r => r.RemoverAsync(animal), Times.Once);
    }

    [Fact]
    public async Task RemoverAsync_ComAnimalInexistente_DeveLancarRecursoNaoEncontradoExceptionENaoDeveRemover()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ObterPorIdAsync(99)).ReturnsAsync((Animal?)null);

        // Act
        Func<Task> act = async () => await _servico.RemoverAsync(99);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _animalRepositorioMock.Verify(r => r.RemoverAsync(It.IsAny<Animal>()), Times.Never);
    }
}