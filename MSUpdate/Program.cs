
using MSUpdate;
using System.Text.Encodings.Web;
using System.Text.Json;

// set the output path to "C:\temp"
var directory = @"C:\personal\dev\temp";
var azureOutputPath = Path.Combine(directory, "azure-updates.json");
var fabricOutputPath = Path.Combine(directory, "fabric-updates.json");
var foundryOutputPath = Path.Combine(directory, "foundry-updates.json");

using var httpClient = new HttpClient
{
     Timeout = TimeSpan.FromSeconds(20)
};

//var azureRssService = new AzureUpdatesRssService(httpClient);
//var updates = await azureRssService.GetUpdatesAsync();

//var fullOutputPath = Path.GetFullPath(azureOutputPath);
//Directory.CreateDirectory(Path.GetDirectoryName(fullOutputPath)!);

//await using var output = File.Create(fullOutputPath);
//await JsonSerializer.SerializeAsync(
//    output,
//    updates,
//    new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

//Console.WriteLine($"Saved {updates.Count} Azure updates to '{fullOutputPath}'.");



//var foundryRssService = new FoundryUpdatesRssService(httpClient);
//var foundryUpdates = await foundryRssService.GetUpdatesAsync();

//var foundryFullOutputPath = Path.GetFullPath(foundryOutputPath);
//Directory.CreateDirectory(Path.GetDirectoryName(foundryFullOutputPath)!);

//await using var foundryOutput = File.Create(foundryFullOutputPath);
//await JsonSerializer.SerializeAsync(
//    foundryOutput,
//    foundryUpdates,
//    new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

//Console.WriteLine($"Saved {foundryUpdates.Count} Foundry updates to '{foundryFullOutputPath}'.");

var fabricRssService = new FabricUpdatesRssService(httpClient);
var fabricUpdates = await fabricRssService.GetUpdatesAsync();

var fabricFullOutputPath = Path.GetFullPath(fabricOutputPath);
Directory.CreateDirectory(Path.GetDirectoryName(fabricFullOutputPath)!);

await using var fabricOutput = File.Create(fabricFullOutputPath);
await JsonSerializer.SerializeAsync(
    fabricOutput,
    fabricUpdates,
    new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping });

Console.WriteLine($"Saved {fabricUpdates.Count} Fabric updates to '{fabricFullOutputPath}'.");