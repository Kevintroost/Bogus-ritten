using Bogus;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.CommandLine;
using System.CommandLine.Invocation;

public class RitData
{
    [JsonPropertyName("@schema")]
    public string Schema { get; set; } = "1.1";

    [JsonPropertyName("rit")]
    public required Rit Rit { get; set; }
}

public class Rit
{
    public required Team team { get; set; }
    public required string gmsRitId { get; set; }
    public bool afgesloten { get; set; }
    public int inzetNummer { get; set; }
    public required string inzetDatumTijd { get; set; }
    public bool handmatigAangemaakt { get; set; }
    public required gmsWaarden gmsWaarden { get; set; }
}

public class Team
{
    public required string voertuignummer { get; set; }
    public required string verpleegkundige { get; set; }
}

public class gmsWaarden
{
    public required List<kladblokItem> kladblok { get; set; }
    public required string urgentie { get; set; }
    public required Statussen statussen { get; set; }
    public required Adres inzetAdres { get; set; }
    public required bestemmingAdres bestemmingAdres { get; set; }
}

public class kladblokItem
{
    public required string tekst { get; set; }
}

public class Statussen
{
    public DateTime apa { get; set; }
    public DateTime mld { get; set; }
    public DateTime opd { get; set; }
    public DateTime vtr { get; set; }
}

public class Adres
{
    public required string straat { get; set; }
    public required string huisnummer { get; set; }
    public required string postcode { get; set; }
    public required string plaats { get; set; }
    public double latitude { get; set; }
    public double longitude { get; set; }
}

public class bestemmingAdres
{
    public double latitude { get; set; }
    public double longitude { get; set; }
}

class RitDataGenerator
{
    public static int Main(string[] args)
    {

        var rootCommand = new RootCommand
        {
            new Option<int>(
                "--aantalRitten",
                description: "Aantal ritten die gegenereerd moeten worden",
                getDefaultValue: () => 1
            ),
            new Option<string>(
                "--filePath",
                description: "Path naar het bestand waar de ritten in opgeslagen moeten worden",
                getDefaultValue: () => "ritten.txt"
            )
        };


        rootCommand.Handler = CommandHandler.Create<int, string>((aantalRitten, filePath) =>
        {
            GenerateRitten(aantalRitten, filePath);
            return Task.CompletedTask;
        });


        return rootCommand.Invoke(args);
    }

    public static void GenerateRitten(int aantalRitten, string filePath)
    {
        Console.WriteLine($"Genereren van {aantalRitten} ritten naar {filePath}");

        
        if (!filePath.EndsWith(".txt", StringComparison.OrdinalIgnoreCase))
        {
            filePath = Path.ChangeExtension(filePath, ".txt");
        }

        using (StreamWriter writer = new StreamWriter(filePath))
        {
            for (int i = 0; i < aantalRitten; i++)
            {
                var ritData = GenerateFakeRitData();
                string json = JsonSerializer.Serialize(ritData, new JsonSerializerOptions { WriteIndented = false });

                writer.WriteLine(json); 
            }
        }

        Console.WriteLine("Bestand succesvol gegenereerd als tekstbestand met JSON records .");
    }



    static RitData GenerateFakeRitData()
    {
        var faker = new Faker();

        return new RitData
        {
            Rit = new Rit
            {
                team = new Team
                {
                    voertuignummer = faker.Random.Number(10000, 99999).ToString(),
                    verpleegkundige = "#insertname",
                },
                gmsRitId = $"01-{faker.Random.Number(1000, 999999999)}",
                afgesloten = false,
                gmsWaarden = new gmsWaarden
                {
                    kladblok = new List<kladblokItem>
                    {
                        new kladblokItem { tekst = faker.Lorem.Sentence() },
                        new kladblokItem { tekst = faker.Lorem.Sentence() }
                    },
                    urgentie = faker.PickRandom(new[] { "A0", "A1", "A2", "B1", "B2" }),
                    statussen = new Statussen
                    {
                        apa = faker.Date.Recent(),
                        mld = faker.Date.Past(),
                        opd = faker.Date.Future(),
                        vtr = faker.Date.Future()
                    },
                    inzetAdres = new Adres
                    {
                        straat = faker.Address.StreetName(),
                        huisnummer = faker.Address.BuildingNumber(),
                        postcode = faker.Address.ZipCode(),
                        plaats = faker.Address.City(),
                        latitude = faker.Address.Latitude(),
                        longitude = faker.Address.Longitude()
                    },
                    bestemmingAdres = new bestemmingAdres
                    {
                        latitude = faker.Address.Latitude(),
                        longitude = faker.Address.Longitude()
                    }
                },
                inzetNummer = faker.Random.Number(100000, 999999),
                inzetDatumTijd = faker.Date.Future().ToString("yyyy-MM-ddTHH:mm:ss"),
                handmatigAangemaakt = faker.Random.Bool()
            }
        };
    }


}
