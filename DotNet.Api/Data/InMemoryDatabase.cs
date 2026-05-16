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

    public static List<CareEvent> CareEvents { get; } = new()
    {
        new CareEvent
        {
            Id = 1,
            PetId = 1,
            Type = "VACCINE",
            Title = "Vacina anual",
            Description = "Aplicação da vacina anual do Thor.",
            ScheduledDate = new DateTime(2026, 6, 10),
            CompletedDate = null,
            Status = "PENDING",
            Priority = "HIGH",
            Notes = "Tutor deve levar carteira de vacinação."
        },
        new CareEvent
        {
            Id = 2,
            PetId = 1,
            Type = "CHECKUP",
            Title = "Check-up respiratório",
            Description = "Avaliação preventiva por conta da raça Pug.",
            ScheduledDate = new DateTime(2026, 5, 1),
            CompletedDate = null,
            Status = "OVERDUE",
            Priority = "CRITICAL",
            Notes = "Pugs podem exigir atenção respiratória preventiva."
        },
        new CareEvent
        {
            Id = 3,
            PetId = 2,
            Type = "MEDICATION",
            Title = "Controle de medicação",
            Description = "Acompanhamento de medicação prescrita para Luna.",
            ScheduledDate = new DateTime(2026, 5, 20),
            CompletedDate = null,
            Status = "PENDING",
            Priority = "MEDIUM",
            Notes = "Confirmar adesão ao tratamento com o tutor."
        }
    };
}