using DotNet.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace DotNet.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public DbSet<Responsavel> Responsaveis => Set<Responsavel>();

    public DbSet<Animal> Animais => Set<Animal>();

    public DbSet<CareEvent> CareEvents => Set<CareEvent>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Responsavel>(entity =>
        {
            entity.ToTable("T_CP_RESPONSAVEIS");

            entity.HasKey(r => r.Id);

            entity.Property(r => r.Id)
                .HasColumnName("ID");

            entity.Property(r => r.Name)
                .HasColumnName("NAME")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(r => r.Email)
                .HasColumnName("EMAIL")
                .HasMaxLength(160)
                .IsRequired();

            entity.Property(r => r.Phone)
                .HasColumnName("PHONE")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(r => r.Cpf)
                .HasColumnName("CPF")
                .HasMaxLength(14)
                .IsRequired();

            entity.Property(r => r.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(r => r.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();
        });

        modelBuilder.Entity<Animal>(entity =>
        {
            entity.ToTable("T_CP_ANIMAIS");

            entity.HasKey(a => a.Id);

            entity.Property(a => a.Id)
                .HasColumnName("ID");

            entity.Property(a => a.ResponsavelId)
                .HasColumnName("RESPONSAVEL_ID")
                .IsRequired();

            entity.Property(a => a.Name)
                .HasColumnName("NAME")
                .HasMaxLength(120)
                .IsRequired();

            entity.Property(a => a.Nickname)
                .HasColumnName("NICKNAME")
                .HasMaxLength(120);

            entity.Property(a => a.Species)
                .HasColumnName("SPECIES")
                .HasMaxLength(30)
                .IsRequired();

            entity.Property(a => a.Breed)
                .HasColumnName("BREED")
                .HasMaxLength(80)
                .IsRequired();

            entity.Property(a => a.BirthDate)
                .HasColumnName("BIRTH_DATE")
                .IsRequired();

            entity.Property(a => a.Weight)
                .HasColumnName("WEIGHT")
                .HasPrecision(10, 2)
                .IsRequired();

            entity.Property(a => a.Sex)
                .HasColumnName("SEX")
                .HasMaxLength(20)
                .IsRequired();

            entity.Property(a => a.Rga)
                .HasColumnName("RGA")
                .HasMaxLength(30);

            entity.Property(a => a.CreatedAt)
                .HasColumnName("CREATED_AT")
                .IsRequired();

            entity.Property(a => a.IsActive)
                .HasColumnName("IS_ACTIVE")
                .HasColumnType("NUMBER(1)")
                .HasConversion<int>()
                .IsRequired();

            // FK: Animal → Responsavel
            entity.HasOne<Responsavel>()
                .WithMany()
                .HasForeignKey(a => a.ResponsavelId)
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

            // FK: CareEvent → Animal
            entity.HasOne<Animal>()
                .WithMany()
                .HasForeignKey(e => e.PetId)
                .OnDelete(DeleteBehavior.Restrict);
        });
    }
}