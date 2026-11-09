using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Models;

[Index(nameof(XipId), IsUnique = true)]
public class Animal {
    public int AnimalId { get; set; }

    [MaxLength(50)]
    public string Nom { get; set; } = "";

    public Especie Especie { get; set; }

    public bool EsFemella { get; set; }

    public DateTime DataNaixement { get; set; }

    public DateTime DataEntrada { get; set; } = DateTime.UtcNow;

    [Range(0.1, 100)]
    public double Pes { get; set; }

    public bool EstaAdoptat { get; set; }

    public DateTime? DataAdopcio { get; set; }

    [Precision(8, 2)]
    public decimal QuotaAdopcio { get; set; }

    public int XipId { get; set; }
    public virtual Xip Xip { get; set; } = null!; // <- "null!" means that we tell the compiler the value will be initialized at runtime
}
