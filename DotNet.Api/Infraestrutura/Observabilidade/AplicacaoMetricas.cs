using System.Diagnostics;
using System.Diagnostics.Metrics;

namespace DotNet.Api.Infraestrutura.Observabilidade;

/// <summary>
/// Ponto central de instrumentação manual de OpenTelemetry: nome do serviço,
/// ActivitySource para Spans de Tracing e Meter/Counter para Métricas customizadas.
/// Registrado como AddSource/AddMeter em Program.cs e usado diretamente nos Controllers.
/// </summary>
public static class AplicacaoMetricas
{
    public const string NomeServico = "ClyvoVet.Api";

    public static readonly ActivitySource ActivitySource = new(NomeServico);

    private static readonly Meter Meter = new(NomeServico);

    /// <summary>
    /// Contador incrementado a cada cadastro (Responsavel, Animal) ou evento de cuidado
    /// registrado com sucesso ou falha, com tags de "recurso" e "status".
    /// </summary>
    public static readonly Counter<long> CadastrosRealizados = Meter.CreateCounter<long>(
        name: "clyvovet.cadastros.total",
        unit: "{cadastro}",
        description: "Total de cadastros e eventos de cuidado processados pela API, com tags de recurso e status.");
}