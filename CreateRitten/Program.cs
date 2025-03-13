using System.Text;
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

public class Program
{
    private static readonly HttpClient _httpClient = new HttpClient();

    public static async Task Main(string[] args)
    {

        var rootCommand = new RootCommand
        {
            new Option<string>(
                "--filePath",
                description: "Route naar de text file die aangemaakt moet worden",
                getDefaultValue: () => "ritten.txt"
            )
        };

        rootCommand.Handler = CommandHandler.Create<string>(CreateRitten);


        await rootCommand.InvokeAsync(args);
    }

    public static async Task CreateRitten(string filePath)
{
    if (!File.Exists(filePath))
    {
        Console.WriteLine("Bestand niet gevonden.");
        return;
    }

    List<RitData> ritDataList = new List<RitData>();

    try
    {
        string[] lines = await File.ReadAllLinesAsync(filePath);

        foreach (var line in lines)
        {
            if (!string.IsNullOrWhiteSpace(line))
            {
                try
                {
                    var ritData = JsonSerializer.Deserialize<RitData>(line);
                    if (ritData != null)
                    {
                        ritDataList.Add(ritData);
                    }
                }
                catch (JsonException ex)
                {
                    Console.WriteLine($"Invalid JSON record: {ex.Message}");
                }
            }
        }
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Fout bij het lezen van het bestand: {ex.Message}");
        return;
    }

    if (ritDataList.Count == 0)
    {
        Console.WriteLine("Geen geldige ritten gevonden in het bestand.");
        return;
    }

    string? authToken = await LoginAsync("#username", "#password");

    if (!string.IsNullOrEmpty(authToken))
    {
        var tasks = ritDataList.Select(rit => CreateRitAsync(rit, authToken));
        bool[] results = await Task.WhenAll(tasks);

        for (int i = 0; i < results.Length; i++)
        {
            Console.WriteLine(results[i] ? $"Rit {i + 1} aangemaakt." : $"Niet succesvol aangemaakt {i + 1}.");
        }
    }
    else
    {
        Console.WriteLine("Login mislukt.");
    }
}


    private static async Task<string?> LoginAsync(string username, string password)
    {
        var loginUrl = "#loginurl";
        var loginRequest = new { grant_type = "password", username, password, clientid = "1" };

        var options = new JsonSerializerOptions { PropertyNamingPolicy = JsonNamingPolicy.CamelCase, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
        var loginJson = JsonSerializer.Serialize(loginRequest, options);
        var content = new StringContent(loginJson, Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync(loginUrl, content);
            string jsonResponse = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var loginData = JsonSerializer.Deserialize<JsonElement>(jsonResponse);
                if (loginData.TryGetProperty("access_token", out var token))
                {
                    return token.GetString();
                }
            }

            Console.WriteLine($"Login mislukt. Status: {response.StatusCode}, Response: {jsonResponse}");
            return null;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error bij het inloggen: {ex.Message}");
            return null;
        }
    }

    private static async Task<bool> CreateRitAsync(RitData ritData, string authToken)
    {
        try
        {
            var request = new HttpRequestMessage(HttpMethod.Post, "#youreventspage");
            request.Headers.Add("Authorization", $"Bearer {authToken}");

            string ritJson = JsonSerializer.Serialize(ritData);
            var content = new StringContent(ritJson, Encoding.UTF8, "application/json");
            request.Content = content;

            var response = await _httpClient.SendAsync(request);
            Console.WriteLine(await response.Content.ReadAsStringAsync());

            return response.IsSuccessStatusCode;
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.StackTrace);
            Console.WriteLine($"Error bij het aanmaken van een rit: {ex.Message}");
            return false;
        }
    }
}
