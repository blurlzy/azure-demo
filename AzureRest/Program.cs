
// scope for client credential flow
using Azure.Core;
using Azure.Identity;
using ConsoleApp1.Models;
using System.Net.Http.Headers;
using System.Text.Json;

string[] ArmScopes = ["https://management.azure.com/.default"];
string AzRestRootEndpoint = "https://management.azure.com";


// load these values from azure key vault
var tenantId = SecretManager.GetSecret("ZLTenantId");
var subscriptionId = SecretManager.GetSecret("ZLSubscriptionId");
var clientId =  SecretManager.GetSecret("AzureRestApiClientId");
var clientSecret = SecretManager.GetSecret("AzureRestApiClientSecret");


if (string.IsNullOrWhiteSpace(tenantId) ||
     string.IsNullOrWhiteSpace(clientId) ||
     string.IsNullOrWhiteSpace(clientSecret) ||
     string.IsNullOrWhiteSpace(subscriptionId))
{
     Console.Error.WriteLine("Missing environment variables. Please set:");
     Console.Error.WriteLine("  AZURE_TENANT_ID, AZURE_CLIENT_ID, AZURE_CLIENT_SECRET, AZURE_SUBSCRIPTION_ID");
     return ;
}

// 1) Acquire ARM token using ClientSecretCredential
var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
AccessToken token = await credential.GetTokenAsync(new TokenRequestContext(ArmScopes));


// 2) Call ARM REST API to list subscription locations
var apiVersion = "2022-12-01";
var url = $"{AzRestRootEndpoint}/subscriptions/{subscriptionId}/locations?api-version={apiVersion}";

using var http = new HttpClient();
http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token.Token);
http.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

using var resp = await http.GetAsync(url);
var body = await resp.Content.ReadAsStringAsync();

if (!resp.IsSuccessStatusCode)
{
     Console.Error.WriteLine($"Request failed: {(int)resp.StatusCode} {resp.ReasonPhrase}");
     Console.Error.WriteLine(body);
     return;
}

// 3) Parse and print regions
var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
var locationsResponse = JsonSerializer.Deserialize<LocationsResponse>(body, options);

if (locationsResponse?.Value == null || locationsResponse.Value.Length == 0)
{
     Console.WriteLine("No locations returned.");
     return;
}

Console.WriteLine($"Found {locationsResponse.Value.Length} locations for subscription {subscriptionId}:");
Console.WriteLine("---------------------------------------------------------------");
Console.WriteLine($"{"name",-20} {"displayName",-30} {"regionType",-10} {"geography",-15}");
Console.WriteLine("---------------------------------------------------------------");

foreach (var loc in locationsResponse.Value.OrderBy(l => l.Name))
{
     var regionType = loc.Metadata?.RegionType ?? "";
     var geography = loc.Metadata?.Geography ?? "";
     Console.WriteLine($"{loc.Name,-20} {loc.DisplayName,-30} {regionType,-10} {geography,-15}");
}