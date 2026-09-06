using Serilog.Context;

namespace DotNet.Api.Aplicacao.Middlewares;

/// <summary>
/// Lê o cabeçalho X-Correlation-ID da requisição (ou gera um novo, se ausente),
/// devolve o mesmo valor na resposta e injeta o valor no LogContext do Serilog,
/// para que todo log emitido durante a requisição carregue essa chave de rastreabilidade.
/// </summary>
public class CorrelationIdMiddleware
{
    private const string HeaderName = "X-Correlation-ID";

    private readonly RequestDelegate _next;

    public CorrelationIdMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var correlationId = context.Request.Headers.TryGetValue(HeaderName, out var valorExistente) &&
                             !string.IsNullOrWhiteSpace(valorExistente)
            ? valorExistente.ToString()
            : Guid.NewGuid().ToString();

        context.Response.OnStarting(() =>
        {
            context.Response.Headers[HeaderName] = correlationId;
            return Task.CompletedTask;
        });

        using (LogContext.PushProperty("CorrelationId", correlationId))
        {
            await _next(context);
        }
    }
}