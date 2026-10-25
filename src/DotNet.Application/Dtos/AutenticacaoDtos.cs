namespace DotNet.Application.Dtos;

public record LoginRequest(string Usuario, string Senha);

public record TokenResponse(string AccessToken, string TokenType, DateTime ExpiraEm);