using System;

namespace AnimalShelterApp.Models;

public class Visita {
    public int VisitaId { get; set; }

    public Motiu Motiu { get; set; }

    public int Cost { get; set; }

    public virtual required Animal Animal { get; set; }

    public DateTime Data { get; set; }

    public Visita() {

    }
}
