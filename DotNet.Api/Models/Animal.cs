namespace DotNet.Api.Models;

public class Animal
{
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
}