using DotNet.Domain.Excecoes;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Aplicacao.Middlewares;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<GlobalExceptionMiddleware> _logger;

    public GlobalExceptionMiddleware(RequestDelegate next, ILogger<GlobalExceptionMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            await TratarAsync(context, ex);
        }
    }

    private async Task TratarAsync(HttpContext context, Exception ex)
    {
        var (status, titulo) = ex switch
        {
            RecursoNaoEncontradoException => (StatusCodes.Status404NotFound, "Recurso não encontrado"),
            RegraDeNegocioException => (StatusCodes.Status400BadRequest, "Regra de negócio violada"),
            _ => (StatusCodes.Status500InternalServerError, "Erro interno do servidor")
        };

        if (status >= 500)
            _logger.LogError(ex, "Erro inesperado ao processar a requisição.");
        else
            _logger.LogWarning("Requisição rejeitada: {Mensagem}", ex.Message);

        var problema = new ProblemDetails
        {
            Status = status,
            Title = titulo,
            Detail = status >= 500 ? "Ocorreu um erro inesperado." : ex.Message,
            Instance = context.Request.Path
        };
        problema.Extensions["traceId"] = context.TraceIdentifier;

        context.Response.StatusCode = status;
        await context.Response.WriteAsJsonAsync(problema, (System.Text.Json.JsonSerializerOptions?)null, "application/problem+json");
    }
}