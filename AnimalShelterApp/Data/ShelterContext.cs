using AnimalShelterApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Data;

public class ShelterContext : DbContext {
    public DbSet<Animal> Animals => Set<Animal>();
    public DbSet<Xip> Xips => Set<Xip>();

    protected override void OnConfiguring(DbContextOptionsBuilder options) =>
        options.UseLazyLoadingProxies().UseSqlite("Data Source=shelter.db");

    protected override void OnModelCreating(ModelBuilder modelBuilder) {
        modelBuilder.Entity<Animal>()
            .HasOne(animal => animal.Xip)
            .WithOne()
            .HasForeignKey<Animal>(animal => animal.XipId)
            .IsRequired();
    }
}
