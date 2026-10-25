using DotNet.Application.Dtos;
using DotNet.Application.Servicos;
using DotNet.Domain.Consultas;
using DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;
using DotNet.Domain.Repositorios;
using FluentAssertions;
using Moq;
using Xunit;

namespace DotNet.Api.UnitTests.Servicos;

public class RelatoTutorServicoTests
{
    private const string IdExistente = "6720f1a3b9e4c2d1a0f3e7b2";

    private static readonly DateTime DataOcorrencia = new(2026, 10, 24, 20, 0, 0, DateTimeKind.Utc);

    private readonly Mock<IRelatoTutorRepositorio> _relatoRepositorioMock;
    private readonly Mock<IAnimalRepositorio> _animalRepositorioMock;
    private readonly RelatoTutorServico _servico;

    public RelatoTutorServicoTests()
    {
        _relatoRepositorioMock = new Mock<IRelatoTutorRepositorio>();
        _animalRepositorioMock = new Mock<IAnimalRepositorio>();
        _servico = new RelatoTutorServico(_relatoRepositorioMock.Object, _animalRepositorioMock.Object);
    }

    private static RelatoTutor CriarRelato() =>
        RelatoTutor.Criar(1, "Comeu pouco hoje", ["apatia"], DataOcorrencia);

    [Fact]
    public async Task CriarAsync_ComAnimalExistente_DeveAdicionarNoRepositorio()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ExisteAsync(1)).ReturnsAsync(true);

        // Act
        var relato = await _servico.CriarAsync(new CriarRelatoTutorRequest(1, "Comeu pouco hoje", DataOcorrencia, ["Apatia"]));

        // Assert
        relato.AnimalId.Should().Be(1);
        relato.Sintomas.Should().Equal("apatia");
        _relatoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<RelatoTutor>()), Times.Once);
    }

    [Fact]
    public async Task CriarAsync_ComAnimalInexistente_DeveLancarRegraDeNegocioExceptionENaoDeveAdicionar()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _servico.CriarAsync(new CriarRelatoTutorRequest(99, "Comeu pouco hoje", DataOcorrencia));

        // Assert
        await act.Should().ThrowAsync<RegraDeNegocioException>();
        _relatoRepositorioMock.Verify(r => r.AdicionarAsync(It.IsAny<RelatoTutor>()), Times.Never);
    }

    [Fact]
    public async Task ObterPorIdAsync_ComRelatoInexistente_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _relatoRepositorioMock.Setup(r => r.ObterPorIdAsync("inexistente")).ReturnsAsync((RelatoTutor?)null);

        // Act
        Func<Task> act = async () => await _servico.ObterPorIdAsync("inexistente");

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
    }

    [Fact]
    public async Task ListarAsync_ComAnimalInexistenteNoFiltro_DeveLancarRecursoNaoEncontradoException()
    {
        // Arrange
        _animalRepositorioMock.Setup(r => r.ExisteAsync(99)).ReturnsAsync(false);

        // Act
        Func<Task> act = async () => await _servico.ListarAsync(new FiltroRelatoTutor(AnimalId: 99), new ParametrosPaginacao());

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _relatoRepositorioMock.Verify(
            r => r.ListarPaginadoAsync(It.IsAny<FiltroRelatoTutor>(), It.IsAny<ParametrosPaginacao>()), Times.Never);
    }

    [Fact]
    public async Task ListarAsync_SemFiltro_DeveConverterItensParaResponse()
    {
        // Arrange
        var paginacao = new ParametrosPaginacao();
        _relatoRepositorioMock
            .Setup(r => r.ListarPaginadoAsync(It.IsAny<FiltroRelatoTutor>(), paginacao))
            .ReturnsAsync(new ResultadoPaginado<RelatoTutor>([CriarRelato()], 1, 10, 1));

        // Act
        var resultado = await _servico.ListarAsync(new FiltroRelatoTutor(), paginacao);

        // Assert
        resultado.TotalItens.Should().Be(1);
        resultado.Itens.Should().ContainSingle().Which.Texto.Should().Be("Comeu pouco hoje");
        _animalRepositorioMock.Verify(r => r.ExisteAsync(It.IsAny<int>()), Times.Never);
    }

    [Fact]
    public async Task AtualizarAsync_ComRelatoExistente_DeveSubstituirNoRepositorio()
    {
        // Arrange
        var relato = CriarRelato();
        _relatoRepositorioMock.Setup(r => r.ObterPorIdAsync(IdExistente)).ReturnsAsync(relato);

        // Act
        await _servico.AtualizarAsync(IdExistente, new AtualizarRelatoTutorRequest("Melhorou à tarde", DataOcorrencia));

        // Assert
        relato.Texto.Should().Be("Melhorou à tarde");
        _relatoRepositorioMock.Verify(r => r.AtualizarAsync(relato), Times.Once);
    }

    [Fact]
    public async Task AtualizarAsync_ComRelatoInexistente_DeveLancarRecursoNaoEncontradoExceptionENaoDeveAtualizar()
    {
        // Arrange
        _relatoRepositorioMock.Setup(r => r.ObterPorIdAsync(IdExistente)).ReturnsAsync((RelatoTutor?)null);

        // Act
        Func<Task> act = async () => await _servico.AtualizarAsync(IdExistente, new AtualizarRelatoTutorRequest("Melhorou", DataOcorrencia));

        // Assert
        await act.Should().ThrowAsync<RecursoNaoEncontradoException>();
        _relatoRepositorioMock.Verify(r => r.AtualizarAsync(It.IsAny<RelatoTutor>()), Times.Never);
    }

    [Fact]
    public async Task RemoverAsync_ComRelatoExistente_DeveRemoverDoRepositorio()
    {
        // Arrange
        var relato = CriarRelato();
        _relatoRepositorioMock.Setup(r => r.ObterPorIdAsync(IdExistente)).ReturnsAsync(relato);

        // Act
        await _servico.RemoverAsync(IdExistente);

        // Assert
        _relatoRepositorioMock.Verify(r => r.RemoverAsync(relato), Times.Once);
    }
}
