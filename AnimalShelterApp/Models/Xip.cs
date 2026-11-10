using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Models;

[Index(nameof(Codi), IsUnique = true)]
public class Xip {
    public int XipId { get; set; }

    [StringLength(15, MinimumLength = 15)]
    public string Codi { get; set; } = "";

    public DateTime DataImplantacio { get; set; } = DateTime.UtcNow;

    public virtual Animal Animal { get; set; } = null!; // <- "null!" means that we tell the compiler the value will be initialized at runtime
}
