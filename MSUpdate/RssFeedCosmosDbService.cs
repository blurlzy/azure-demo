
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace MSUpdate;

public sealed class RssFeedCosmosDbService
{
     private readonly Container _container;

     public RssFeedCosmosDbService(Container container)
     {
          _container = container;
     }

     public async Task<IReadOnlyList<CosmosItem>> GetItemsAsync(
         string source,
         DateTimeOffset from,
         DateTimeOffset to,
         CancellationToken cancellationToken = default)
     {
          ArgumentException.ThrowIfNullOrWhiteSpace(source);

          if (from >= to)
          {
               throw new ArgumentException("The start date must be earlier than the end date.", nameof(from));
          }

          var query = new QueryDefinition(
              "SELECT * FROM c WHERE c.source = @source AND c.publishedAt >= @from AND c.publishedAt <= @to")
              .WithParameter("@source", source)
              .WithParameter("@from", from.ToUniversalTime().ToString("O"))
              .WithParameter("@to", to.ToUniversalTime().ToString("O"));

          using var iterator = _container.GetItemQueryIterator<CosmosItem>(query);
          var items = new List<CosmosItem>();

          while (iterator.HasMoreResults)
          {
               var response = await iterator.ReadNextAsync(cancellationToken);
               items.AddRange(response);
          }

          return items;
     }

     // sync azure updates
     public async Task SaveAzureUpdatesAsync(IEnumerable<AzureUpdate> updates, CancellationToken cancellationToken = default)
     {
          // get min and max published date from the updates
          // convert to date time offset to ensure the correct time zone is used
          // if its null, set as current date time offset to ensure the correct time zone is used
          // minus 1 day to ensure we get all updates in case of time zone differences
          var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
          var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;

          // load the existing updates from the db with date range
          var existingUpdates = await GetItemsAsync(UpdateSources.Azure, minPublishedDate, maxPublishedDate, cancellationToken);

          // 
          foreach (var azureUpdate in updates)
          {
               // check updated date, if its the same, skip it
               var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == azureUpdate.Id);

               // check if it already exists by rss item id
               if (existingUpdate != null && existingUpdate.UpdatedAt == azureUpdate.UpdatedAt)
               {
                    continue;
               }

               // save the new update to the db
               var item = new CosmosItem(
                     azureUpdate.Id,
                     UpdateSources.Azure,
                     azureUpdate.Link,
                     azureUpdate.Title,
                     azureUpdate.Description,
                     azureUpdate.Categories.ToArray(),
                     string.Empty,
                     azureUpdate.PublishedAt,
                     azureUpdate.UpdatedAt
               );

               // save into cosmos db
               await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
          }

     }

     // save foundry updates
     public async Task SaveFoundryUpdatesAsync(IEnumerable<FoundryUpdate> updates, CancellationToken cancellationToken = default)
     {
          // get min and max published date from the updates
          var minPublishedDate = updates.Min(u => u.PublishedAt)?.AddDays(-1).ToUniversalTime() ?? DateTimeOffset.UtcNow;
          var maxPublishedDate = updates.Max(m => m.PublishedAt)?.ToUniversalTime() ?? DateTimeOffset.UtcNow;
          // load the existing updates from the db with date range
          var existingUpdates = await GetItemsAsync(UpdateSources.Foundry, minPublishedDate, maxPublishedDate, cancellationToken);

          foreach (var foundryUpdate in updates)
          {
               // check updated date, if its the same, skip it
               var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == foundryUpdate.Id);
               // check if it already exists by rss item id
               if (existingUpdate != null && existingUpdate.UpdatedAt == foundryUpdate.UpdatedAt)
               {
                    continue;
               }

               // save the new update to the db
               var item = new CosmosItem(
                     foundryUpdate.Id,
                     UpdateSources.Foundry,
                     foundryUpdate.Link,
                     foundryUpdate.Title,
                     Util.ExtractFirstParagraph(foundryUpdate.Description), // only save the first paragraph of the description
                     Array.Empty<string>(),
                     foundryUpdate.Creator,
                     foundryUpdate.PublishedAt,
                     foundryUpdate.UpdatedAt
               );
               // save into cosmos db
               await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
          }
     }

     // save github updates
     public async Task SaveGitHubUpdatesAsync(IEnumerable<GitHubFeedItem> updates, CancellationToken cancellationToken = default)
     {
          if (!updates.Any())
          {
               return;
          }

          // get min and max published date from the updates
          var minPublishedDate = updates.Min(u => u.PublishedAt).AddDays(-1).ToUniversalTime();
          var maxPublishedDate = updates.Max(m => m.PublishedAt).ToUniversalTime();

          // load the existing updates from the db with date range
          var existingUpdates = await GetItemsAsync(UpdateSources.GitHub, minPublishedDate, maxPublishedDate, cancellationToken);

          foreach(var githubUpdate in updates)
          {
               // check updated date, if its the same, skip it
               var existingUpdate = existingUpdates.FirstOrDefault(e => e.RssItemId == githubUpdate.Id);
               // check if it already exists by rss item id
               if (existingUpdate != null && existingUpdate.UpdatedAt == githubUpdate.PublishedAt)
               {
                    continue;
               }
               // save the new update to the db
               var item = new CosmosItem(
                     githubUpdate.Id,
                     UpdateSources.GitHub,
                     githubUpdate.Url.AbsoluteUri,
                     githubUpdate.Title,
                     githubUpdate.Summary ?? string.Empty,
                     githubUpdate.Categories.ToArray(),
                     string.Empty,
                     githubUpdate.PublishedAt,
                     githubUpdate.PublishedAt
               );

               // save into cosmos db
               await _container.CreateItemAsync(item, new PartitionKey(item.Partition));
          }
     }

}
