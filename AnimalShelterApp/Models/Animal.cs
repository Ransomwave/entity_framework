using System.ComponentModel.DataAnnotations;
using System.Text.Json;
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

    public virtual HashSet<Voluntari> Voluntaris { get; set; }

    public virtual Animal? Mare { get; set; }

    public virtual HashSet<Animal> Cries { get; set; }

    public virtual HashSet<Visita> Visitas { get; set; }

    [Precision(8, 2)]
    public decimal DespesaAcumulada { get; set; }

    public Animal() {
        Voluntaris = [];
        Cries = [];
        Visitas = [];
        DataNaixement = DateTime.UtcNow;
        EstaAdoptat = false;
        QuotaAdopcio = 0;
        DespesaAcumulada = 0;
    }

    public override string ToString() {
        return $"AnimalId: {AnimalId}, Nom: {Nom}, Especie: {Especie}, EsFemella: {EsFemella}, Pes: {Pes}, DataNaixement: {DataNaixement}, DataEntrada: {DataEntrada}, EstaAdoptat: {EstaAdoptat}, DataAdopcio: {DataAdopcio}, QuotaAdopcio: {QuotaAdopcio}, Xip: {Xip.Codi}";
    }


}
