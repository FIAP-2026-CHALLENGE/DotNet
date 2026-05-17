using DotNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Tutor> Tutors => Set<Tutor>();

    public DbSet<Pet> Pets => Set<Pet>();

    public DbSet<CareEvent> CareEvents => Set<CareEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Tutor>(entity =>
        {
            entity.ToTable("T_CP_TUTORS");

            entity.HasKey(t => t.Id);

            entity.Property(t => t.Id)
                .HasColumnName("ID");

            entity.Property(t => t.Name)
                .HasColumnName("NAME")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(t => t.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(t => t.Phone)
                .HasColumnName("PHONE")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(t => t.Cpf)
                .HasColumnName("CPF")
                .HasMaxLength(14)
                .IsRequired();

            entity.Property(t => t.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(t => t.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();
        });

        modelBuilder.Entity<Pet>(entity =>
        {
            entity.ToTable("T_CP_PETS");

            entity.HasKey(p => p.Id);

            entity.Property(p => p.Id)
                .HasColumnName("ID");

            entity.Property(p => p.TutorId)
                .HasColumnName("TUTOR_ID")
                .IsRequired();

            entity.Property(p => p.Name)
                .HasColumnName("NAME")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(p => p.Nickname)
                .HasColumnName("NICKNAME")
                .HasMaxLength(120);

            entity.Property(p => p.Species)
                .HasColumnName("SPECIES")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(p => p.Breed)
                .HasColumnName("BREED")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(p => p.BirthDate)
                .HasColumnName("BIRTH_DATE")
                .IsRequired();

            entity.Property(p => p.Weight)
                .HasColumnName("WEIGHT")
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(p => p.Sex)
                .HasColumnName("SEX")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(p => p.Rga)
                .HasColumnName("RGA")
                .HasMaxLength(30);

            entity.Property(p => p.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(p => p.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();

            // FK: Pet → Tutor
            entity.HasOne<Tutor>()
                .WithMany()
                .HasForeignKey(p => p.TutorId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        modelBuilder.Entity<CareEvent>(entity =>
        {
            entity.ToTable("T_CP_CARE_EVENTS");

            entity.HasKey(e => e.Id);

            entity.Property(e => e.Id)
                .HasColumnName("ID");

            entity.Property(e => e.PetId)
                .HasColumnName("PET_ID")
                .IsRequired();

            entity.Property(e => e.Type)
                .HasColumnName("TYPE")
                .HasMaxLength(40)
                .IsRequired();

            entity.Property(e => e.Title)
                .HasColumnName("TITLE")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(e => e.Description)
                .HasColumnName("DESCRIPTION")
                .HasMaxLength(500);

            entity.Property(e => e.ScheduledDate)
                .HasColumnName("SCHEDULED_DATE")
                .IsRequired();

            entity.Property(e => e.CompletedDate)
                .HasColumnName("COMPLETED_DATE");

            entity.Property(e => e.Status)
                .HasColumnName("STATUS")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Priority)
                .HasColumnName("PRIORITY")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(e => e.Notes)
                .HasColumnName("NOTES")
                .HasMaxLength(500);

            entity.Property(e => e.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(e => e.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();

            // FK: CareEvent → Pet
            entity.HasOne<Pet>()
                .WithMany()
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}