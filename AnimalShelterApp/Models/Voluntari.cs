using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Models;

[Index(nameof(Nom), IsUnique = true)]
public class Voluntari {
    public int VoluntariId { get; set; }

    [MaxLength(50)]
    public required string Nom { get; set; }

    public Torn Torn { get; set; }

    public int MinutsPasseig { get; set; }

    public virtual ICollection<Animal> Animals { get; set; }

    public Voluntari() {
        MinutsPasseig = 0;
        Animals = new HashSet<Animal>();
    }

    public override string ToString() {
        return $"{VoluntariId}. {Nom} ({Torn}) - {MinutsPasseig} minuts de passeig";
    }
}
