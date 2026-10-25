namespace DotNet.Domain.Entidades;
using DotNet.Domain.Excecoes;

public class Responsavel
{
    public int Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Phone { get; set; } = string.Empty;

    public string Cpf { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    /// <summary>
    /// Cria um novo Responsavel validando as regras de domínio.
    /// Lança <see cref="RegraDeNegocioException"/> quando algum dado obrigatório é inválido.
    /// </summary>
    public static Responsavel Criar(string name, string email, string phone, string cpf)
    {
        ValidarDados(name, email, phone, cpf);

        return new Responsavel
        {
            Name = name.Trim(),
            Email = email.Trim(),
            Phone = phone.Trim(),
            Cpf = cpf.Trim(),
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    /// <summary>
    /// Atualiza os dados de um Responsavel já existente, revalidando as regras de domínio.
    /// </summary>
    public void Atualizar(string name, string email, string phone, string cpf, bool isActive)
    {
        ValidarDados(name, email, phone, cpf);

        Name = name.Trim();
        Email = email.Trim();
        Phone = phone.Trim();
        Cpf = cpf.Trim();
        IsActive = isActive;
    }

    private static void ValidarDados(string name, string email, string phone, string cpf)
    {
        if (string.IsNullOrWhiteSpace(name))
            throw new RegraDeNegocioException("O nome é obrigatório.");

        if (string.IsNullOrWhiteSpace(email))
            throw new RegraDeNegocioException("O e-mail é obrigatório.");

        if (string.IsNullOrWhiteSpace(phone))
            throw new RegraDeNegocioException("O telefone é obrigatório.");

        if (string.IsNullOrWhiteSpace(cpf))
            throw new RegraDeNegocioException("O CPF é obrigatório.");
    }
}