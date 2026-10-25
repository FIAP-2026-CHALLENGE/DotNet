using DotNet.Api.Aplicacao;
using DotNet.Api.Hateoas;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DotNet.Api.Controllers;

/// <summary>
/// Raiz da API: nome, versão e ambiente, com links para os recursos principais.
/// </summary>
[AllowAnonymous]
[ApiController]
[Route("")]
[Produces("application/json")]
public class InfoController : ControllerBase
{
    private readonly IWebHostEnvironment _ambiente;

    public InfoController(IWebHostEnvironment ambiente)
    {
        _ambiente = ambiente;
    }

    [HttpGet]
    [ProducesResponseType(typeof(Recurso<InformacoesApiResponse>), 200)]
    public ActionResult<Recurso<InformacoesApiResponse>> Get()
    {
        var informacoes = new InformacoesApiResponse(InformacoesApi.Nome, InformacoesApi.Versao, _ambiente.EnvironmentName);

        return Ok(new Recurso<InformacoesApiResponse>(informacoes,
        [
            new("login", Url.Action("Login", "Auth")!, "POST"),
            new("responsaveis", Url.Action("GetAll", "Responsaveis")!, "GET"),
            new("animais", Url.Action("GetAll", "Animais")!, "GET"),
            new("care-events", Url.Action("GetAll", "CareEvents")!, "GET"),
            new("relatos", Url.Action("GetAll", "Relatos")!, "GET"),
            new("health", "/health", "GET"),
            new("health-mongo", "/health/mongo", "GET")
        ]));
    }
}
