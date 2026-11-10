using System.Diagnostics;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;
using AnimalShelterApp.Services;

using var ctx = new ShelterContext();
ctx.Database.EnsureDeleted();
ctx.Database.EnsureCreated();

var animals = new List<Animal> {
    new() { Nom = "Foo", Especie = Especie.GOS, EsFemella = false, Pes = 28.5, DataNaixement = new(2019, 3, 12), DataEntrada = new(2023, 1, 10), Xip = new() { Codi = "000000000000001" } },
    new() { Nom = "Bar", Especie = Especie.GOS, EsFemella = true, Pes = 18.2, DataNaixement = new(2020, 7, 4), DataEntrada = new(2023, 6, 21), Xip = new() { Codi = "000000000000002" } },
    new() { Nom = "Baz", Especie = Especie.GOS, EsFemella = false, Pes = 35.0, DataNaixement = new(2017, 11, 30), DataEntrada = new(2022, 9, 3), Xip = new() { Codi = "000000000000003" } },
    new() { Nom = "Qux", Especie = Especie.GOS, EsFemella = true,  Pes = 9.8, DataNaixement = new(2022, 1, 18), DataEntrada = new(2024, 2, 14), Xip = new() { Codi = "000000000000004" } },
    new() { Nom = "Michi", Especie = Especie.GAT, EsFemella = false, Pes = 4.3, DataNaixement = new(2021, 5, 9), DataEntrada = new(2023, 11, 2), Xip = new() { Codi = "000000000000005" } },
    new() { Nom = "Kira", Especie = Especie.GAT, EsFemella = true,  Pes = 3.6, DataNaixement = new(2018, 8, 25), DataEntrada = new(2024, 4, 30), Xip = new() { Codi = "000000000000006" } },
    new() { Nom = "Simba", Especie = Especie.GAT, EsFemella = false, Pes = 5.1, DataNaixement = new(2023, 2, 7), DataEntrada = new(2024, 8, 19), Xip = new() { Codi = "000000000000007" } },
    new() { Nom = "Bolet", Especie = Especie.CONILL, EsFemella = true,  Pes = 1.9, DataNaixement = new(2023, 6, 15), DataEntrada = new(2024, 10, 5), Xip = new() { Codi = "000000000000008" } },
    new() { Nom = "Piolet", Especie = Especie.AU, EsFemella = false, Pes = 0.3, DataNaixement = new(2022, 9, 1), DataEntrada = new(2025, 1, 12), Xip = new() { Codi = "000000000000009" } },
};

ctx.Animals.AddRange(animals);
ctx.SaveChanges();

Console.WriteLine("=== ANIMALS ===");
foreach (var animal in ctx.Animals.ToList()) {
    Console.WriteLine($"{animal.Nom} ({animal.Especie}) Xip: {animal.Xip.Codi}");
}

// AnimalService tests
Console.WriteLine("=== AnimalService ===");
var animalService = new AnimalService(ctx);

var availableDogs = animalService.GetDisponibles(Especie.GOS);
foreach (var dog in availableDogs) {
    Console.WriteLine($"Available dog: {dog.Nom} ({dog.Xip.Codi})");
}

// will find first
var foundAnimal = animalService.CercaPerXip("000000000000001");
Console.WriteLine($"Found animal: {foundAnimal?.Nom} ({foundAnimal?.Xip.Codi})");

// won't find anything
animalService.CercaPerXip("000000000006767");

// extra
animalService.CercaPerXip("67");
animalService.CercaPerXip("sixseven67");

var foundByName = animalService.CercaPerNom("mich");
foreach (var animal in foundByName) {
    Console.WriteLine($"Found by name: {animal.Nom} ({animal.Xip.Codi})");
}

// === PART 2 ===
Debug.WriteLine("=== PART 2 ===");
var adoptantService = new AdoptantService(ctx);

// assuming it wants a hardcoded adopted animal
var dogToAdopt = availableDogs[0];
dogToAdopt.EstaAdoptat = true;
dogToAdopt.DataAdopcio = DateTime.UtcNow;

var adoptants = new List<Adoptant> {
    new() { DNI = "12345678Z", Nom = "Marta", Cognoms = "Puig Soler", TeJardi = true, Animals = [dogToAdopt] },
    new() { DNI = "87654321X", Nom = "Pau", Cognoms = "Ferrer Vidal", TeJardi = false },
};

var marta = adoptants[0];
var pau = adoptants[1];

ctx.Adoptants.AddRange(adoptants);
ctx.SaveChanges();


adoptantService.Adoptar(marta.DNI, availableDogs[1].Xip.Codi);

// errors
adoptantService.Adoptar(pau.DNI, availableDogs[2].Xip.Codi); // doesn't have yard
adoptantService.Adoptar("67676767A", availableDogs[2].Xip.Codi); // adptant doesn't exist
adoptantService.Adoptar(marta.DNI, "676767676767"); // animal doesn't exist

// extra (2)
adoptantService.Adoptar(marta.DNI, "000000000000005");
adoptantService.Adoptar(marta.DNI, "000000000000006"); // fails, max animals per person is 3!

adoptantService.Retornar("000000000000005");
adoptantService.Retornar("000000000000005"); // trying again will fail (not adopted)

foreach (var animal in marta.Animals) {
    Debug.WriteLine($"Animal Marta: {animal.Xip.Codi}");
}

// extra (3)
adoptantService.LlistaAdoptants(1);