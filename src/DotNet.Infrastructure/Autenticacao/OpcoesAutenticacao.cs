namespace DotNet.Infrastructure.Autenticacao;

public class JwtOpcoes
{
    public const string Secao = "Jwt";

    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiraEmMinutos { get; set; } = 60;
}

public class CredenciaisAdminOpcoes
{
    public const string Secao = "Auth";

    public string Usuario { get; set; } = string.Empty;
    public string Senha { get; set; } = string.Empty;
}