using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Models;

[Index(nameof(DNI), IsUnique = true)]
public class Adoptant {
    public int AdoptantId { get; set; }

    [StringLength(9)]
    public required string DNI { get; set; }

    [StringLength(50)]
    public required string Nom { get; set; }

    [StringLength(100)]
    public required string Cognoms { get; set; }

    public required bool TeJardi { get; set; }

    public DateTime DataRegister { get; set; }

    public virtual ICollection<Animal> Animals { get; set; }

    public Adoptant() {
        DataRegister = DateTime.UtcNow;
        Animals = new HashSet<Animal>();
    }
}
