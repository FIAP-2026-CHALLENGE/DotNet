namespace DotNet.Api.Models;

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
    /// Lança <see cref="ArgumentException"/> quando algum dado obrigatório é inválido.
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
            throw new ArgumentException("Name is required.", nameof(name));

        if (string.IsNullOrWhiteSpace(email))
            throw new ArgumentException("Email is required.", nameof(email));

        if (string.IsNullOrWhiteSpace(phone))
            throw new ArgumentException("Phone is required.", nameof(phone));

        if (string.IsNullOrWhiteSpace(cpf))
            throw new ArgumentException("Cpf is required.", nameof(cpf));
    }
}