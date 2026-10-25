namespace DotNet.Api.Aplicacao;

/// <summary>
/// Identificação da API, exibida no título do Swagger e em GET /.
/// Na demonstração de CI/CD, altere a Versao e faça o commit: o novo valor aparece no app publicado.
/// </summary>
public static class InformacoesApi
{
    public const string Nome = "CLYVO VET API";

    public const string Versao = "1.0.0";

    public const string Descricao =
        "Monitoramento e cuidado de cães e gatos: responsáveis, animais e eventos de cuidado no Oracle, " +
        "e relatos do tutor no MongoDB.";
}

public record InformacoesApiResponse(string Nome, string Versao, string Ambiente);
