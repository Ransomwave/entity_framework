using AnimalShelterApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Data;

public class ShelterContext : DbContext {
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Xip> Xips => Set<Xip>();
    public DbSet<Adoptant> Adoptants => Set<Adoptant>();

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
            .HasForeignKey("AdoptantID")
            .IsRequired(false);
    }
}
