using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Models;

public class Animal {

    public int AnimalId { get; set; }

    [MaxLength(50)]
    public required string Nom { get; set; }

    public Especie Especie { get; set; }

    public bool EsFemella { get; set; }

    public DateTime DataNaixement { get; set; }

    public DateTime DataEntrada { get; set; }

    [Range(0.1, 100)]
    public double Pes { get; set; }

    public bool EstaAdoptat { get; set; }

    public DateTime? DataAdopcio { get; set; }

    [Precision(8, 2)]
    public decimal QuotaAdopcio { get; set; }

    public virtual required Xip Xip { get; set; }

    public virtual Adoptant? Adoptant { get; set; }

    public Animal() {
        DataNaixement = DateTime.UtcNow;
        EstaAdoptat = false;
        QuotaAdopcio = 0;
    }
}
