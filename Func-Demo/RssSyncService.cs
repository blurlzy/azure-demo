using Func_Demo.Auth;
using Func_Demo.Persistence;
using Func_Demo.Rss;
using Microsoft.Azure.Cosmos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;

namespace Func_Demo;

public class RssSyncService
{
     // http client
     private readonly HttpClient _httpClient;
     // rss services
     private readonly AzureRssService _azureRssService;
     private readonly FoundryRssService _foundryRssService;
     // cosmos db service
     private readonly CosmosDataService _cosmosDataService;

     private readonly ILogger _logger;

     // ctor
     public RssSyncService(ILoggerFactory loggerFactory)
     {
          _logger = loggerFactory.CreateLogger<RssSyncService>();

          // logging
          _logger.LogInformation("Initializing Comsmos Data service....");

          // cosmos connection
          string cosmosConnection = SecretManager.GetSecret(SecretKeys.CosmosConnection);
          string cosmosDb = SecretManager.GetSecret(SecretKeys.CosmosDb);
          string containerName = SecretManager.GetSecret(SecretKeys.CosmosContainer);

          // Configure JsonSerializerOptions
          var options = new CosmosClientOptions
          {
               SerializerOptions = new CosmosSerializationOptions
               {
                    PropertyNamingPolicy = CosmosPropertyNamingPolicy.CamelCase
               },
          };
          var cosmosClient = new CosmosClient(cosmosConnection, options);
          var cosmosContainer = cosmosClient.GetContainer(cosmosDb, containerName);

          _cosmosDataService = new CosmosDataService(cosmosContainer);

          // logging
          _logger.LogInformation("Initializing RSS service....");

          _httpClient = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };

          _azureRssService = new AzureRssService(_httpClient);
          _foundryRssService = new FoundryRssService(_httpClient);
     }


     // This function will be triggered every day at 12:00 AM and 12:00 PM UTC
     // it syncs the RSS feeds for Azure, Foundry, and other MS platform / product updates 
     [Function("RssSyncService")]
     public async Task Run([TimerTrigger("0 0 0,12 * * *")] TimerInfo myTimer)
     {
          _logger.LogInformation("C# Timer trigger function executed at: {executionTime}", DateTime.Now);
          _logger.LogInformation("Loading Azure updates...");

          var azureUpdates = await _azureRssService.GetUpdatesAsync();

          _logger.LogInformation("Saving Azure updates...");

          await _cosmosDataService.SaveAzureUpdatesAsync(azureUpdates);

          _logger.LogInformation("Loading Foundry updates...");
          var foundryUpdates = await _foundryRssService.GetUpdatesAsync();

          _logger.LogInformation("Saving Foundry updates...");
          await _cosmosDataService.SaveFoundryUpdatesAsync(foundryUpdates);

          if (myTimer.ScheduleStatus is not null)
          {
               _logger.LogInformation("Next timer schedule at: {nextSchedule}", myTimer.ScheduleStatus.Next);
          }
     }
}