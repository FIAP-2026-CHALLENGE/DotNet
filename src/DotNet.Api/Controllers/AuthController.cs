using DotNet.Application.Dtos;
using DotNet.Application.Servicos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

[ApiController]
[Route("api/auth")]
[Produces("application/json")]
[AllowAnonymous]
public class AuthController : ControllerBase
{
    private readonly IAutenticacaoServico _servico;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAutenticacaoServico servico, ILogger<AuthController> logger)
    {
        _servico = servico;
        _logger = logger;
    }

    [HttpPost("login")]
    [ProducesResponseType(typeof(TokenResponse), 200)]
    [ProducesResponseType(401)]
    public ActionResult<TokenResponse> Login(LoginRequest request)
    {
        var token = _servico.Autenticar(request);

        _logger.LogInformation("Login realizado com sucesso para o usuário {Usuario}.", request.Usuario);

        return Ok(token);
    }
}