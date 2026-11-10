using System;
using System.Diagnostics;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Services;

public class AdoptantService(ShelterContext context) {
    private readonly ShelterContext _context = context;

    public bool Adoptar(string dni, string chipCode) {
        var adoptant = _context.Adoptants.FirstOrDefault(adoptant => adoptant.DNI == dni);
        if (adoptant == null) {
            Debug.WriteLine($"Couldn't find adoptant with DNI: {dni}");
            return false;
        }

        var animal = _context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);
        if (animal == null) {
            Debug.WriteLine($"Couldn't find animal with chip: {chipCode}");
            return false;
        }

        if (animal.Especie == Especie.GOS && adoptant.TeJardi == false) {
            Debug.WriteLine($"Adopting a dog requires a yard!");
            return false;
        }

        // extra: max 3 animals
        if (adoptant.Animals.Count == 3) {
            Debug.WriteLine($"You can only adopt up to 3 animals!");
            return false;
        }

        animal.EstaAdoptat = true;
        animal.DataAdopcio = DateTime.UtcNow;

        adoptant.Animals.Add(animal);

        _context.SaveChanges();

        Debug.WriteLine($"{adoptant.Nom} successfully adopted {animal.Nom}");
        return true;
    }

    public bool Retornar(string chipCode) {
        var animal = _context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);
        if (animal == null) {
            Debug.WriteLine($"Couldn't find animal with chip: {chipCode}");
            return false;
        }

        if (animal.EstaAdoptat == false || animal.Adoptant == null) {
            Debug.WriteLine($"Animal with chip {chipCode} is not adopted");
            return false;
        }

        animal.Adoptant.Animals.Remove(animal);
        animal.Adoptant = null;
        animal.EstaAdoptat = false;
        animal.DataAdopcio = null;

        Debug.WriteLine($"Success: Returned animal with chip {chipCode}");

        _context.SaveChanges();

        return true;
    }

    public void LlistaAdoptants(int minAnimals) {
        var query = from adoptant in _context.Adoptants
                    where adoptant.Animals.Count >= minAnimals
                    orderby adoptant.Animals.Count
                    select adoptant;

        var resultList = query.ToList();

        Debug.WriteLine("Adoptants list:");
        foreach (var item in resultList) {
            Debug.WriteLine($"{item.Nom}, ({item.Animals.Count})");
        }

    }

}
