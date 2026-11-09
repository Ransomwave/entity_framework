using System.Diagnostics;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;
using Microsoft.EntityFrameworkCore;

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
        // extra
        if (chipCode.Length != 15 || !chipCode.All(char.IsDigit)) {
            Debug.WriteLine($"Invalid chip code '{chipCode}'");
            return null;

        }

        var animal = _context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);

        if (animal == null) {
            Debug.WriteLine($"Animal with xip '{chipCode}' not found");
            return null;
        }

        return animal;
    }

    public List<Animal> CercaPerNom(string name) {
        return _context.Animals
            // use EF.Functions.Like instead of turning into list to avoid copying all animals into memory
            .Where(animal => EF.Functions.Like(animal.Nom, $"%{name}%"))
            .ToList();
    }


}