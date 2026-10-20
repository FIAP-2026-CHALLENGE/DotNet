namespace DotNet.Api.Models;

public class Animal
{
    public static readonly string[] AllowedSpecies =
    {
        "DOG",
        "CAT"
    };

    public int Id { get; set; }

    public int ResponsavelId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Nickname { get; set; } = string.Empty;

    public string Species { get; set; } = string.Empty;

    public string Breed { get; set; } = string.Empty;

    public DateTime BirthDate { get; set; }

    public decimal Weight { get; set; }

    public string Sex { get; set; } = string.Empty;

    public string Rga { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public bool IsActive { get; set; } = true;

    public static Animal Criar(
        int responsavelId,
        string name,
        string nickname,
        string species,
        string breed,
        DateTime birthDate,
        decimal weight,
        string sex,
        string rga)
    {
        var normalizedSpecies = ValidarDados(responsavelId, name, species, breed, sex, weight);

        return new Animal
        {
            ResponsavelId = responsavelId,
            Name = name.Trim(),
            Nickname = nickname?.Trim() ?? string.Empty,
            Species = normalizedSpecies,
            Breed = breed.Trim(),
            BirthDate = birthDate,
            Weight = weight,
            Sex = sex.Trim().ToUpper(),
            Rga = rga?.Trim() ?? string.Empty,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
    }

    public void Atualizar(
        int responsavelId,
        string name,
        string nickname,
        string species,
        string breed,
        DateTime birthDate,
        decimal weight,
        string sex,
        string rga,
        bool isActive)
    {
        var normalizedSpecies = ValidarDados(responsavelId, name, species, breed, sex, weight);

        ResponsavelId = responsavelId;
        Name = name.Trim();
        Nickname = nickname?.Trim() ?? string.Empty;
        Species = normalizedSpecies;
        Breed = breed.Trim();
        BirthDate = birthDate;
        Weight = weight;
        Sex = sex.Trim().ToUpper();
        Rga = rga?.Trim() ?? string.Empty;
        IsActive = isActive;
    }

    private static string ValidarDados(int responsavelId, string name, string species, string breed, string sex, decimal weight)
    {
        if (responsavelId <= 0 ||
            string.IsNullOrWhiteSpace(name) ||
            string.IsNullOrWhiteSpace(species) ||
            string.IsNullOrWhiteSpace(breed) ||
            string.IsNullOrWhiteSpace(sex) ||
            weight <= 0)
        {
            throw new ArgumentException("ResponsavelId, name, species, breed, sex and valid weight are required.");
        }

        var normalizedSpecies = species.Trim().ToUpper();

        if (!AllowedSpecies.Contains(normalizedSpecies))
        {
            throw new ArgumentException("Species must be DOG or CAT in this MVP version.", nameof(species));
        }

        return normalizedSpecies;
    }
}