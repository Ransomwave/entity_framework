using AnimalShelterApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Data;

public class ShelterContext : DbContext {
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Xip> Xips => Set<Xip>();
    public DbSet<Adoptant> Adoptants => Set<Adoptant>();
    public DbSet<Voluntari> Voluntaris => Set<Voluntari>();
    public DbSet<Visita> Visitas => Set<Visita>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseLazyLoadingProxies().UseSqlite("Data Source=shelter.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Animal>()
            .HasOne(animal => animal.Xip)
            .WithOne(xip => xip.Animal)
            .HasForeignKey<Animal>("XipId")
            .IsRequired()
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Animal>()
            .HasOne(animal => animal.Adoptant) // an animal can only have 1 adoptant
            .WithMany(adoptant => adoptant.Animals) // an adoptant can have many animals
            .HasForeignKey("AdoptantId")
            .IsRequired(false);

        modelBuilder.Entity<Animal>()
            .HasOne(animal => animal.Mare)
            .WithMany(animal => animal.Cries)
            .HasForeignKey("MareId")
            .IsRequired(false);

        modelBuilder.Entity<Voluntari>()
            .HasMany(voluntari => voluntari.Animals)
            .WithMany(animal => animal.Voluntaris)
            .UsingEntity(join => join.ToTable("Cura"));

        modelBuilder.Entity<Voluntari>()
            .Property(voluntari => voluntari.Torn)
            .HasConversion<string>();

        modelBuilder.Entity<Visita>()
            .Property(v => v.Motiu)
            .HasConversion<string>();

        modelBuilder.Entity<Visita>()
            .HasOne(visita => visita.Animal)
            .WithMany(animal => animal.Visitas)
            .OnDelete(DeleteBehavior.Cascade);

    }
}
