using System.Diagnostics;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;

namespace AnimalShelterApp.Services;

public class VoluntariService(ShelterContext context) {
    private const int MAX_ANIMALS = 4;

    private readonly ShelterContext _context = context;

    public List<Voluntari> GetTots() {
        return _context.Voluntaris.OrderBy(voluntari => voluntari.VoluntariId).ToList();
    }

    public bool AssignarAnimal(string nomVoluntari, string chipCode) {
        var voluntari = _context.Voluntaris.FirstOrDefault(voluntari => voluntari.Nom == nomVoluntari);
        if (voluntari == null) {
            Debug.WriteLine($"Couldn't find voluntari with name: {nomVoluntari}");
            return false;
        }

        var animal = _context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);
        if (animal == null) {
            Debug.WriteLine($"Couldn't find animal with chip: {chipCode}");
            return false;
        }

        if (animal.EstaAdoptat) {
            Debug.WriteLine($"{animal.Nom} is already adopted");
            return false;
        }

        if (voluntari.Animals.Contains(animal)) {
            Debug.WriteLine($"{voluntari.Nom} already takes care of {animal.Nom}");
            return false;
        }

        if (voluntari.Animals.Count >= MAX_ANIMALS) {
            Debug.WriteLine($"{voluntari.Nom} already takes care of {MAX_ANIMALS} animals");
            return false;
        }

        voluntari.Animals.Add(animal);
        _context.SaveChanges();

        Debug.WriteLine($"{voluntari.Nom} now takes care of {animal.Nom}");
        return true;
    }

    public bool DesassignarAnimal(string nomVoluntari, string chipCode) {
        var voluntari = _context.Voluntaris.FirstOrDefault(voluntari => voluntari.Nom == nomVoluntari);
        if (voluntari == null) {
            Debug.WriteLine($"Couldn't find voluntari with name: {nomVoluntari}");
            return false;
        }

        var animal = _context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);
        if (animal == null) {
            Debug.WriteLine($"Couldn't find animal with chip: {chipCode}");
            return false;
        }

        if (!voluntari.Animals.Contains(animal)) {
            Debug.WriteLine($"{voluntari.Nom} doesn't take care of {animal.Nom}");
            return false;
        }

        voluntari.Animals.Remove(animal);
        _context.SaveChanges();

        Debug.WriteLine($"{voluntari.Nom} no longer takes care of {animal.Nom}");
        return true;
    }

    public bool RegistrarPasseig(string nomVoluntari, string chipCode, int minuts) {
        var voluntari = _context.Voluntaris.FirstOrDefault(voluntari => voluntari.Nom == nomVoluntari);
        if (voluntari == null) {
            Debug.WriteLine($"Couldn't find voluntari with name: {nomVoluntari}");
            return false;
        }

        var animal = _context.Animals.FirstOrDefault(animal => animal.Xip.Codi == chipCode);
        if (animal == null) {
            Debug.WriteLine($"Couldn't find animal with chip: {chipCode}");
            return false;
        }

        if (!voluntari.Animals.Contains(animal)) {
            Debug.WriteLine($"{voluntari.Nom} doesn't take care of {animal.Nom}");
            return false;
        }

        if (animal.Especie != Especie.GOS) {
            Debug.WriteLine($"{animal.Nom} is a {animal.Especie}, only dogs can be walked");
            return false;
        }

        var previousMinutes = voluntari.MinutsPasseig;
        voluntari.MinutsPasseig += minuts;
        _context.SaveChanges();

        Debug.WriteLine($"{voluntari.Nom} walked {animal.Nom} for {minuts} minutes: {previousMinutes} -> {voluntari.MinutsPasseig}");
        return true;
    }
}
