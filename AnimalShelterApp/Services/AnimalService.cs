using System.Diagnostics;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;

namespace AnimalShelterApp.Services;

public class AnimalService(ShelterContext context) {

    private readonly ShelterContext _context = context;

    public List<Animal> GetDisponibles(Especie species) {
        var query = from animal in _context.Animals
                    where animal.Especie == species && !animal.EstaAdoptat
                    orderby animal.DataEntrada descending
                    select animal;

        return query.ToList();
    }

    public Animal? CercaPerXip(string chipCode) {
        var animal = _context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);

        if (animal == null) {
            Debug.WriteLine($"Animal with xip '{chipCode}' not found");
            return null;
        }

        return animal;
    }
}