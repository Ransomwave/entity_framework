using System.Diagnostics;
using AnimalShelterApp.Data;
using AnimalShelterApp.Models;
using AnimalShelterApp.Services;

using var ctx = new ShelterContext();
ctx.Database.EnsureDeleted();
ctx.Database.EnsureCreated();

var animals = new List<Animal> {
    new() {
        Nom = "Foo", Especie = Especie.GOS, EsFemella = false, Pes = 28.5, DataNaixement = new(2019, 3, 12),
        DataEntrada = new(2023, 1, 10), Xip = new() { Codi = "000000000000001" }
    },
    new() {
        Nom = "Bar", Especie = Especie.GOS, EsFemella = true, Pes = 18.2, DataNaixement = new(2020, 7, 4),
        DataEntrada = new(2023, 6, 21), Xip = new() { Codi = "000000000000002" }
    },
    new() {
        Nom = "Baz", Especie = Especie.GOS, EsFemella = false, Pes = 35.0, DataNaixement = new(2017, 11, 30),
        DataEntrada = new(2022, 9, 3), Xip = new() { Codi = "000000000000003" }
    },
    new() {
        Nom = "Qux", Especie = Especie.GOS, EsFemella = true, Pes = 9.8, DataNaixement = new(2022, 1, 18),
        DataEntrada = new(2024, 2, 14), Xip = new() { Codi = "000000000000004" }
    },
    new() {
        Nom = "Michi", Especie = Especie.GAT, EsFemella = false, Pes = 4.3, DataNaixement = new(2021, 5, 9),
        DataEntrada = new(2023, 11, 2), Xip = new() { Codi = "000000000000005" }
    },
    new() {
        Nom = "Kira", Especie = Especie.GAT, EsFemella = true, Pes = 3.6, DataNaixement = new(2018, 8, 25),
        DataEntrada = new(2024, 4, 30), Xip = new() { Codi = "000000000000006" }
    },
    new() {
        Nom = "Simba", Especie = Especie.GAT, EsFemella = false, Pes = 5.1, DataNaixement = new(2023, 2, 7),
        DataEntrada = new(2024, 8, 19), Xip = new() { Codi = "000000000000007" }
    },
    new() {
        Nom = "Bolet", Especie = Especie.CONILL, EsFemella = true, Pes = 1.9, DataNaixement = new(2023, 6, 15),
        DataEntrada = new(2024, 10, 5), Xip = new() { Codi = "000000000000008" }
    },
    new() {
        Nom = "Piolet", Especie = Especie.AU, EsFemella = false, Pes = 0.3, DataNaixement = new(2022, 9, 1),
        DataEntrada = new(2025, 1, 12), Xip = new() { Codi = "000000000000009" }
    },
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
Console.WriteLine("=== PART 2 ===");
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
    Console.WriteLine($"Animal Marta: {animal.Xip.Codi}");
}

// extra (3)
adoptantService.LlistaAdoptants(1);

// === PART 3 ===
Console.WriteLine("=== PART 3 ===");
var baz = ctx.Animals.First(animal => animal.Xip.Codi == "000000000000003");
var qux = ctx.Animals.First(animal => animal.Xip.Codi == "000000000000004");
var kira = ctx.Animals.First(animal => animal.Xip.Codi == "000000000000006");

var voluntaris = new List<Voluntari> {
    new() { Nom = "Aina", Torn = Torn.MATI, Animals = [baz, qux] },
    new() { Nom = "Joan", Torn = Torn.TARDA, Animals = [kira] },
    new() { Nom = "Nuria", Torn = Torn.CAP_DE_SETMANA },
};

ctx.Voluntaris.AddRange(voluntaris);
ctx.SaveChanges();

var voluntariService = new VoluntariService(ctx);

void PrintVoluntaris() {
    foreach (var voluntari in voluntariService.GetTots()) {
        Console.WriteLine(voluntari);
        foreach (var animal in voluntari.Animals) {
            Console.WriteLine($"   {animal.Nom} ({animal.Xip.Codi})");
        }
    }
}

PrintVoluntaris();

voluntariService.AssignarAnimal("Nuria", "000000000000005");
voluntariService.AssignarAnimal("Pere", "000000000000005"); // voluntari doesn't exist
voluntariService.AssignarAnimal("Aina", "000000000000999"); // animal doesn't exist
voluntariService.AssignarAnimal("Aina", "000000000000001"); // animal is adopted
voluntariService.AssignarAnimal("Aina", "000000000000003"); // already assigned
voluntariService.AssignarAnimal("Aina", "000000000000007");
voluntariService.AssignarAnimal("Aina", "000000000000009");
voluntariService.AssignarAnimal("Aina", "000000000000006"); // fails, max animals per voluntari is 4!

voluntariService.DesassignarAnimal("Aina", "000000000000009");
voluntariService.DesassignarAnimal("Pere", "000000000000009"); // voluntari doesn't exist
voluntariService.DesassignarAnimal("Aina", "000000000000999"); // animal doesn't exist
voluntariService.DesassignarAnimal("Nuria", "000000000000003"); // not assigned

// adopting clears the animal's voluntaris
adoptantService.Adoptar(marta.DNI, "000000000000003");
Console.WriteLine($"Voluntaris of Baz: {baz.Voluntaris.Count}");
PrintVoluntaris();

voluntariService.RegistrarPasseig("Aina", "000000000000004", 30);
voluntariService.RegistrarPasseig("Aina", "000000000000004", 45); // minutes accumulate
voluntariService.RegistrarPasseig("Joan", "000000000000006", 20); // not a dog
voluntariService.RegistrarPasseig("Nuria", "000000000000004", 20); // not assigned

PrintVoluntaris();

// === PART 4 ===
Console.WriteLine("=== PART 4 ===");

var nala = new Animal() {
    Nom = "Nala", Especie = Especie.GOS, EsFemella = true, Pes = 25,
    DataNaixement = DateTime.Today.AddYears(-9), Xip = new Xip() { Codi = "724098100000011" }
};
var coco = new Animal() {
    Nom = "Coco", Especie = Especie.GOS, EsFemella = true, Pes = 21,
    DataNaixement = DateTime.Today.AddYears(-5), Xip = new Xip() { Codi = "724098100000012" }
};
var rocky = new Animal() {
    Nom = "Rocky", Especie = Especie.GOS, EsFemella = false, Pes = 6.5,
    DataNaixement = DateTime.Today.AddMonths(-4), Xip = new Xip() { Codi = "724098100000013" }
};
var bimba = new Animal() {
    Nom = "Bimba", Especie = Especie.GOS, EsFemella = true, Pes = 5.8,
    DataNaixement = DateTime.Today.AddMonths(-4), Xip = new Xip() { Codi = "724098100000014" }
};

coco.Cries.Add(rocky);
coco.Cries.Add(bimba);
nala.Cries.Add(coco);

var animalsWithChildren = new List<Animal> { nala, coco, rocky, bimba };

ctx.Animals.Add(nala);
ctx.SaveChanges();

foreach (var animal in animalsWithChildren) {
    Console.WriteLine(
        $"{animal.Nom}:\n\tMare: {(animal.Mare != null ? animal.Mare : "Sense mare")}\n\tCries: [{string.Join(" -- ", animal.Cries)}]");
}

// 1
// La Bimba (cria de la Coco), la Nala (gossa, mare de la Coco) i el Rocky (gos, mascle)
Console.WriteLine("\n--- Canviar la mare ---");
animalService.CanviarMare("724098100000014", "724098100000011"); // bé: la Bimba ara és cria de la Nala
animalService.CanviarMare("724098100000014", "724098100000011"); // false: ja té la Nala com a mare
animalService.CanviarMare("724098100000014", null); // bé: la Bimba es queda sense mare
animalService.CanviarMare("724098100000014", "724098100000013"); // false: el Rocky és mascle
animalService.CanviarMare("724098100000999", "724098100000011"); // false: no hi ha cap cria amb aquest xip
animalService.CanviarMare("724098100000014", "724098100000999"); // false: no hi ha cap mare amb aquest xip
animalService.CanviarMare("724098100000014", null); // false: la Bimba ja no té mare

// 2
Console.WriteLine("\n--- Registrar cries ---");
var toby = new Animal() {
    Nom = "Toby", Especie = Especie.GOS, EsFemella = false, Pes = 2.1,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000015" }
};
animalService.RegistrarCria("724098100000012", toby);               // bé: el Toby és cria de la Coco

var criaThor = new Animal() {
    Nom = "Max", Especie = Especie.GOS, EsFemella = false, Pes = 2,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000016" }
};
animalService.RegistrarCria("724098100000013", criaThor);           // false: el Rocky és mascle

var criaGat = new Animal() {
    Nom = "Misha", Especie = Especie.GAT, EsFemella = true, Pes = 0.8,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000017" }
};
animalService.RegistrarCria("724098100000012", criaGat);            // false: la Coco és un gos i la Misha un gat

var criaXip = new Animal() {
    Nom = "Duc", Especie = Especie.GOS, EsFemella = false, Pes = 2,
    DataNaixement = DateTime.Today.AddMonths(-1), Xip = new Xip() { Codi = "724098100000011" }
};
animalService.RegistrarCria("724098100000012", criaXip);            // false: el xip ja és de la Nala

// Altres return false (l'enunciat no els demana al Program)
var criaVella = new Animal() {
    Nom = "Avi", Especie = Especie.GOS, EsFemella = false, Pes = 20,
    DataNaixement = DateTime.Today.AddYears(-10), Xip = new Xip() { Codi = "724098100000018" }
};
animalService.RegistrarCria("724098100000012", criaVella);          // false: ha nascut abans que la Coco
animalService.RegistrarCria("724098100000999", criaVella);          // false: no hi ha cap mare amb aquest xip

// 3
Console.WriteLine("\n--- Descendents ---");
Console.WriteLine(string.Join(", ", animalService.Descendants("724098100000011")));

// === PART 5 ===
Console.WriteLine("=== PART 5 ===");

Console.WriteLine("\n -- 1 --");

// should be double (it is)
// animalService.CalcularDespesaMes();
// animalService.CalcularDespesaMes();

Console.WriteLine("\n -- 2 --");

// Animal Nala: Previous expense: 0, New expense: 85.80, Total accumulated expense: 85.80
animalService.CalcularDespesaMes();

// adopt Coco
// new adopter to not mess up the code above
var johnDoe = new Adoptant { DNI = "67676767Z", Nom = "John", Cognoms = "Doe", TeJardi = true };
ctx.Adoptants.Add(johnDoe);
ctx.SaveChanges();

Console.WriteLine("-- Coco -> John Doe --");

// Animal Nala: Previous expense: 85.80, New expense: 78.0, Total accumulated expense: 163.80 
adoptantService.Adoptar("67676767Z", "724098100000012");
animalService.CalcularDespesaMes();

