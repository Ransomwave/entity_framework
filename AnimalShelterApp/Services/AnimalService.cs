using System.Diagnostics;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;
using Microsoft.EntityFrameworkCore;

namespace AnimalShelterApp.Services;

public class AnimalService(ShelterContext context) {
    public List<Animal> GetDisponibles(Especie species) {
        var query = from animal in context.Animals
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

        var animal = context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);

        if (animal == null) {
            Debug.WriteLine($"Animal with xip '{chipCode}' not found");
            return null;
        }

        return animal;
    }

    public List<Animal> CercaPerNom(string name) {
        return context.Animals
            // use EF.Functions.Like instead of turning into list to avoid copying all animals into memory
            .Where(animal => EF.Functions.Like(animal.Nom, $"%{name}%"))
            .ToList();
    }

    public bool CanviarMare(string childCode, string? motherCode) {
        var child = context.Animals.FirstOrDefault(animal => animal.Xip.Codi == childCode);
        if (child == null) {
            Console.WriteLine($"Child with xip '{childCode}' not found");
            return false;
        }

        if (motherCode == null) {
            child.Mare = null;
            context.SaveChanges();
            Console.WriteLine($"Removed mother for child {child.Nom}");
            return false;
        }

        var newMother = context.Animals.FirstOrDefault(animal => animal.Xip.Codi == motherCode);
        if (newMother == null) {
            Console.WriteLine($"Mother with chip {motherCode} doesn't exist!");
            return false;
        }

        if (!newMother.EsFemella) {
            Console.WriteLine($"Animal {newMother.Nom} is not female, cannot be mother");
            return false;
        }

        if (child.Mare == newMother) {
            Console.WriteLine($"Child {child.Nom} already has mother {newMother.Nom}");
            return false;
        }

        child.Mare = newMother;
        context.SaveChanges();
        Console.WriteLine($"Set mother for child {child.Nom} to {newMother.Nom}");
        return true;
    }

    public bool RegistrarCria(string motherCode, Animal child) {
        var mother = context.Animals.FirstOrDefault(animal => animal.Xip.Codi == motherCode);
        if (mother == null) {
            Console.WriteLine($"Mother with chip {motherCode} does not exist!");
            return false;
        }

        if (!mother.EsFemella) {
            Console.WriteLine($"Animal {mother.Nom} is not female, cannot have children");
            return false;
        }

        if (child.Especie != mother.Especie) {
            Console.WriteLine($"Child {child.Nom} is of species {child.Especie}, which does not match mother {mother.Nom}'s species {mother.Especie}");
            return false;
        }

        if (child.DataNaixement <= mother.DataNaixement) {
            Console.WriteLine($"Child {child.Nom} has birth date {child.DataNaixement}, which is not after mother {mother.Nom}'s birth date {mother.DataNaixement}");
            return false;
        }

        if (context.Xips.Any(xip => xip.Codi == child.Xip.Codi)) {
            Console.WriteLine($"Chip {child.Xip.Codi} is already in use");
            return false;
        }

        mother.Cries.Add(child);

        context.SaveChanges();

        Console.WriteLine($"Registered child {child.Nom} for mother {mother.Nom}");

        return true;
    }

    private void GetDescendants(Animal animal, List<Animal> descendantList) {
        foreach (var child in animal.Cries) {
            descendantList.Add(child);
            GetDescendants(child, descendantList);
        }
    }

    public List<Animal> Descendants(string chipCode) {
        var animal = context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);
        if (animal == null) {
            Console.WriteLine($"Animal with chip {chipCode} does not exist!");
            return new List<Animal>();
        }

        var descendantList = new List<Animal>();
        GetDescendants(animal, descendantList);
        return descendantList;
    }
}