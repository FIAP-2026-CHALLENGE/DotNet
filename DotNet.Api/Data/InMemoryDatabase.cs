using DotNet.Api.Models;

namespace DotNet.Api.Data;

public static class InMemoryDatabase
{
    public static List<Tutor> Tutors { get; } = new()
    {
        new Tutor
        {
            Id = 1,
            Name = "Ana Souza",
            Email = "ana.souza@email.com",
            Phone = "11999990000",
            Cpf = "12345678900"
        },
        new Tutor
        {
            Id = 2,
            Name = "Carlos Lima",
            Email = "carlos.lima@email.com",
            Phone = "11988887777",
            Cpf = "98765432100"
        }
    };

    public static List<Pet> Pets { get; } = new()
    {
        new Pet
        {
            Id = 1,
            TutorId = 1,
            Name = "Thor",
            Nickname = "Toto",
            Species = "DOG",
            Breed = "Pug",
            BirthDate = new DateTime(2020, 5, 10),
            Weight = 8.5m,
            Sex = "MALE",
            Rga = "RGA123456"
        },
        new Pet
        {
            Id = 2,
            TutorId = 2,
            Name = "Luna",
            Nickname = "Lulu",
            Species = "CAT",
            Breed = "Siamês",
            BirthDate = new DateTime(2021, 8, 20),
            Weight = 4.2m,
            Sex = "FEMALE",
            Rga = "RGA987654"
        }
    };
}