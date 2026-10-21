using DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;
using FluentAssertions;
using Xunit;

namespace DotNet.Api.UnitTests.Dominio;

public class CareEventTests
{
    private static readonly DateTime ScheduledDate = new(2026, 6, 10, 10, 0, 0);

    [Fact]
    public void Criar_ComDadosValidos_DeveCriarEventoPendente()
    {
        // Arrange & Act
        var careEvent = CareEvent.Criar(
            1, "CHECKUP", "Check-up respiratório", "Avaliação preventiva.",
            ScheduledDate, null, "PENDING", "HIGH", null);

        // Assert
        careEvent.Type.Should().Be("CHECKUP");
        careEvent.Status.Should().Be("PENDING");
        careEvent.Priority.Should().Be("HIGH");
        careEvent.CompletedDate.Should().BeNull();
    }

    [Fact]
    public void Criar_ComStatusCompletedSemCompletedDate_DevePreencherCompletedDateAutomaticamente()
    {
        // Arrange & Act
        var careEvent = CareEvent.Criar(
            1, "VACCINE", "Vacina antirrábica", "Dose anual.",
            ScheduledDate, null, "COMPLETED", "MEDIUM", null);

        // Assert
        careEvent.CompletedDate.Should().NotBeNull();
    }

    [Fact]
    public void Criar_ComNotesVazio_DeveGuardarNull()
    {
        // Arrange & Act
        var careEvent = CareEvent.Criar(
            1, "CHECKUP", "Título", "Descrição", ScheduledDate, null, "PENDING", "LOW", "");

        // Assert
        careEvent.Notes.Should().BeNull();
    }

    [Theory]
    [InlineData("EXPLOSION")]
    [InlineData("")]
    public void Criar_ComTipoInvalido_DeveLancarRegraDeNegocioException(string type)
    {
        // Arrange
        Action act = () => CareEvent.Criar(
            1, type, "Título", "Descrição", ScheduledDate, null, "PENDING", "LOW", null);

        // Act & Assert
        act.Should().Throw<RegraDeNegocioException>();
    }

    [Fact]
    public void Concluir_ComEventoPendente_DeveAlterarStatusParaCompleted()
    {
        // Arrange
        var careEvent = CareEvent.Criar(
            1, "CHECKUP", "Título", "Descrição", ScheduledDate, null, "PENDING", "LOW", null);

        // Act
        careEvent.Concluir();

        // Assert
        careEvent.Status.Should().Be("COMPLETED");
        careEvent.CompletedDate.Should().NotBeNull();
    }

    [Fact]
    public void Concluir_ComEventoCancelado_DeveLancarRegraDeNegocioException()
    {
        // Arrange
        var careEvent = CareEvent.Criar(
            1, "CHECKUP", "Título", "Descrição", ScheduledDate, null, "CANCELED", "LOW", null);

        // Act
        Action act = () => careEvent.Concluir();

        // Assert
        act.Should().Throw<RegraDeNegocioException>();
    }
}