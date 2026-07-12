using Microsoft.Azure.Cosmos;
using MSUpdate;
using Xunit.Abstractions;

namespace Demo.Tests
{
     public class CosmosTests
     {
          private readonly string _cosmosConnection = SecretManager.GetSecret(SecretKeys.CosmosConnection);
          private readonly string _cosmosDb = SecretManager.GetSecret(SecretKeys.CosmosDb);
          private readonly string _container = SecretManager.GetSecret(SecretKeys.CosmosContainer);

          // cosmos client
          private readonly CosmosClient _client;
          // services
          private readonly RssFeedCosmosDbService _rssFeedCosmosDbService;
          // http client
          private readonly HttpClient _httpClient;
          // azure updates rss services
          private readonly AzureUpdatesRssService _azureUpdatesRssService;
          // foundry updates rss services
          private readonly FoundryUpdatesRssService _foundryUpdateRssService;
          // github updates rss services
          private readonly GithubUpdatesRssService _githubUpdateRssService;
          // fabric updates rss services
          private readonly FabricUpdatesRssService _fabricUpdateRssService;

          // output
          private readonly ITestOutputHelper _output;

          //ctor
          public CosmosTests(ITestOutputHelper output)
          {
               _output = output;

               // Configure JsonSerializerOptions
               var options = new CosmosClientOptions
               {
                    SerializerOptions = new CosmosSerializationOptions
                    {
                         PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
                    },
               };

               this._client = new CosmosClient(_cosmosConnection, options);
               var container = this._client.GetContainer(_cosmosDb, _container);
               this._rssFeedCosmosDbService = new RssFeedCosmosDbService(container);

               _httpClient = new HttpClient
               {
                    Timeout = TimeSpan.FromSeconds(20)
               };

               _azureUpdatesRssService = new AzureUpdatesRssService(_httpClient);
               _foundryUpdateRssService = new FoundryUpdatesRssService(_httpClient);
               _githubUpdateRssService = new GithubUpdatesRssService(_httpClient);
               _fabricUpdateRssService = new FabricUpdatesRssService(_httpClient);
          }


          [Fact]
          public async Task Save_Azure_Updates_Test()
          {
               var azureUpdates = await _azureUpdatesRssService.GetUpdatesAsync();

               // save into cosmos db
               await _rssFeedCosmosDbService.SaveAzureUpdatesAsync(azureUpdates);
          }

          [Fact]
          public async Task Save_Foundry_Updates_Test()
          {
               var foundryUpdates = await _foundryUpdateRssService.GetUpdatesAsync();

               // save into cosmos db
               await _rssFeedCosmosDbService.SaveFoundryUpdatesAsync(foundryUpdates);
          }

          [Fact]
          public async Task Save_Github_Updates_Test()
          {
               var githubUpdates = await _githubUpdateRssService.GetLatestAsync();

               // save into cosmos db
               await _rssFeedCosmosDbService.SaveGitHubUpdatesAsync(githubUpdates);

               //foreach(var item in githubUpdates)
               //{
               //     _output.WriteLine($"Url: {item.Url.AbsoluteUri}, Title: {item.Title}, PublishedAt: {item.PublishedAt}");
               //}
          }

          [Fact]
          public async Task Save_Fabric_Updates_Test()
          {
               var fabricUpdates = await _fabricUpdateRssService.GetUpdatesAsync();
               foreach (var item in fabricUpdates)
               {
                    _output.WriteLine($"Id: {item.Id}, Title: {item.Title}, Link: {item.Link}, PublishedAt: {item.PublishedAt}");
               }    
          }
     }
}
