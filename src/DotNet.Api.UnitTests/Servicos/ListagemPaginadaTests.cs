using DotNet.Application.Servicos;
using DotNet.Domain.Consultas;
using DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;
using DotNet.Domain.Repositorios;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotNet.Api.UnitTests.Servicos;

public class ListagemPaginadaTests
{
    private readonly Mock<IAnimalRepositorio> _animalRepositorioMock = new();
    private readonly Mock<IResponsavelRepositorio> _responsavelRepositorioMock = new();
    private readonly Mock<ICareEventRepositorio> _careEventRepositorioMock = new();

    private readonly AnimalServico _animalServico;
    private readonly CareEventServico _careEventServico;
    private readonly ResponsavelServico _responsavelServico;

    private static readonly ParametrosPaginacao Paginacao = new();

    public ListagemPaginadaTests()
    {
        _animalServico = new AnimalServico(_animalRepositorioMock.Object, _responsavelRepositorioMock.Object);
        _careEventServico = new CareEventServico(_careEventRepositorioMock.Object, _animalRepositorioMock.Object);
        _responsavelServico = new ResponsavelServico(_responsavelRepositorioMock.Object);
    }

    [Fact]
    public async Task Animal_ListarAsync_SemResponsavelNoFiltro_DeveMapearResultadoENaoConsultarResponsavel()
    {
        // Arrange
        var filtro = new FiltroAnimal(Species: "DOG");
        var thor = Animal.Criar(1, "Thor", "Toto", "DOG", "Pug", new DateTime(2020, 5, 10), 8.5m, "MALE", "RGA123456");

        _animalRepositorioMock
            .Setup(r => r.ListarPaginadoAsync(filtro, Paginacao))
            .ReturnsAsync(new ResultadoPaginado<Animal>(new List<Animal> { thor }, 1, 10, 1));

        // Act
        var resultado = await _animalServico.ListarAsync(filtro, Paginacao);

        // Assert
        resultado.TotalItens.Should().Be(1);
        resultado.Itens.Single().Name.Should().Be("Thor");
        _responsavelRepositorioMock.Verify(r => r.ExisteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task CareEvent_ListarAsync_DeveNormalizarStatusAntesDeConsultar()
    {
        // Arrange
        _careEventRepositorioMock
            .Setup(r => r.ListarPaginadoAsync(It.IsAny<FiltroCareEvent>(), It.IsAny<ParametrosPaginacao>()))
            .ReturnsAsync(new ResultadoPaginado<CareEvent>(new List<CareEvent>(), 1, 10, 0));

        // Act
        await _careEventServico.ListarAsync(new FiltroCareEvent(Status: " pending "), Paginacao);

        // Assert
        _careEventRepositorioMock.Verify(
            r => r.ListarPaginadoAsync(It.Is<FiltroCareEvent>(f => f.Status == "PENDING" && f.Type == null), Paginacao),
            Times.Once);
    }

    [Fact]
    public async Task CareEvent_ListarAsync_ComTypeInvalido_DeveLancarRegraDeNegocioExceptionENaoConsultar()
    {
        // Act
        Func<Task> act = async () => await _careEventServico.ListarAsync(new FiltroCareEvent(Type: "TIPO_INEXISTENTE"), Paginacao);

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _careEventRepositorioMock.Verify(
            r => r.ListarPaginadoAsync(It.IsAny<FiltroCareEvent>(), It.IsAny<ParametrosPaginacao>()),
            Times.Never);
    }

    [Fact]
    public async Task CareEvent_ListarAsync_ComAnimalInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _careEventServico.ListarAsync(new FiltroCareEvent(AnimalId: 99), Paginacao);

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task Responsavel_ListarAsync_DeveRepassarFiltroEPaginacaoAoRepositorio()
    {
        // Arrange
        var filtro = new FiltroResponsavel(Nome: "ana", Ativo: true);

        _responsavelRepositorioMock
            .Setup(r => r.ListarPaginadoAsync(filtro, Paginacao))
            .ReturnsAsync(new ResultadoPaginado<Responsavel>(new List<Responsavel>(), 1, 10, 0));

        // Act
        var resultado = await _responsavelServico.ListarAsync(filtro, Paginacao);

        // Assert
        resultado.Itens.Should().BeEmpty();
        _responsavelRepositorioMock.Verify(r => r.ListarPaginadoAsync(filtro, Paginacao), Times.Once);
    }
}
