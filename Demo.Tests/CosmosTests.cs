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
     }
}
