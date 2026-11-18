using System;
using System.Collections;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;

namespace AnimalShelterApp.Services;

public class VisitaService(ShelterContext context) {

    private readonly Random random = new();

    public void GenerarVisites(int amount) {
        var animals = context.Animals.ToList();
        var animalCount = animals.Count;

        var reasons = Enum.GetValues<Motiu>();

        var visits = new List<Visita>();
        for (int i = 0; i < amount; i++) {
            var randomAnimal = animals[random.Next(animalCount)];

            var randomReason = reasons[random.Next(reasons.Length)];

            // var randomDate = DateTime.Today.AddDays(-random.Next(0, 365));
            var daysBetweenEntryAndToday = (DateTime.Today - randomAnimal.DataEntrada).Days; // extra
            var randomDate = DateTime.Today.AddDays(-random.Next(0, daysBetweenEntryAndToday + 1));

            // var randomCost = random.Next(20, 200);
            var (minCost, maxCost) = randomReason switch {
                Motiu.REVISIO => (30, 60),
                Motiu.VACUNA => (25, 50),
                Motiu.FERIDA => (60, 150),
                Motiu.CIRURGIA => (250, 600)
            };
            var randomCost = random.Next(minCost, maxCost + 1);

            visits.Add(new Visita {
                Motiu = randomReason,
                Cost = randomCost,
                Data = randomDate,
                Animal = randomAnimal
            });

            // animal doesn't need to be updated because the relationship is already set in the Visita object

            Console.WriteLine($"Visit generated: {randomReason} for {randomAnimal.Nom} on {randomDate:d} costing ${randomCost}");
        }

        context.Visitas.AddRange(visits);
        context.SaveChanges();
    }

    public void EstadistiquesPerEspecie() {
        var animalStatsBySpecies = context.Animals
            .GroupBy(animal => animal.Especie)
            .Select(g => new {
                Species = g.Key,
                TotalAnimals = g.Count(),
                Adopted = g.Count(animal => animal.EstaAdoptat == true),
                Available = g.Count(animal => animal.EstaAdoptat == false),
                AverageWeight = g.Average(animal => animal.Pes)
            })
            .OrderByDescending(stats => stats.TotalAnimals);

        foreach (var animal in animalStatsBySpecies) {
            Console.WriteLine($"Species: {animal.Species}, Total: {animal.TotalAnimals}, Adopted: {animal.Adopted}, Available: {animal.Available}, Average Weight: {animal.AverageWeight:F2} kg");
        }
    }

    public void EstadistiquesPerMotiu() {
        var visitStatsByReason = context.Visitas
            .GroupBy(v => v.Motiu)
            .Select(g => new {
                Reason = g.Key,
                Amount = g.Count(),
                TotalCost = g.Sum(v => v.Cost),
                AverageCost = Math.Round(g.Average(v => v.Cost), 2)
            })
            .OrderByDescending(stats => stats.TotalCost);

        // Console.WriteLine($"Resulting query length: {visitStatsByReason.Count()}");

        foreach (var visit in visitStatsByReason) {
            Console.WriteLine($"Reason: {visit.Reason}, Amount: {visit.Amount}, Total Cost: ${visit.TotalCost}, Average Cost: ${visit.AverageCost}");
        }
    }

    public void LlistatVoluntaris(int minAnimals) {
        var volunteers = context.Voluntaris
        .Where(v => v.Animals.Count() >= minAnimals)
        .OrderByDescending(v => v.Animals.Count())
        .Select(result => new {
            Name = result.Nom,
            Shift = result.Torn,
            AnimalCount = result.Animals.Count(),
            Minutes = result.MinutsPasseig
        });

        foreach (var volunteer in volunteers) {
            Console.WriteLine($"{volunteer.Name} ({volunteer.Shift}) - animals: {volunteer.AnimalCount} - {volunteer.Minutes} min");
        }
    }

    public List<Voluntari> VoluntarisDeSeniors() {
        var seniorVolunteers = context.Voluntaris
        .Where(v => v.Animals.Any(a => DateTime.Today.Year - a.DataNaixement.Year >= 65));

        foreach (var volunteer in seniorVolunteers) {
            Console.WriteLine($"{volunteer.Nom} ({volunteer.Torn}) - animals: {volunteer.Animals.Count()}");
        }
        return seniorVolunteers.ToList();

    }
}
