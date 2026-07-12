
using Azure.Core;
using Azure.Identity;
using Microsoft.Azure.Cosmos;

namespace MSUpdate;

public sealed class AzureUpdatesCosmosDbService
{
    private readonly Container _container;

    public AzureUpdatesCosmosDbService(CosmosClient cosmosClient, string databaseId, string containerId)
    {
        ArgumentNullException.ThrowIfNull(cosmosClient);
        ArgumentException.ThrowIfNullOrWhiteSpace(databaseId);
        ArgumentException.ThrowIfNullOrWhiteSpace(containerId);

        _container = cosmosClient.GetContainer(databaseId, containerId);
    }

    public static CosmosClient CreateClient(Uri accountEndpoint, TokenCredential? credential = null)
    {
        ArgumentNullException.ThrowIfNull(accountEndpoint);

        return new CosmosClient(accountEndpoint.AbsoluteUri, credential ?? new DefaultAzureCredential());
    }

    public async Task SaveAsync(
        IEnumerable<AzureUpdate> updates,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(updates);

        //foreach (var update in updates)
        //{
        //    var document = new AzureUpdateDocument(
        //        update.Id,
        //        update.Title,
        //        update.Link,
        //        update.Categories,
        //        update.Description,
        //        update.PublishedAt,
        //        update.UpdatedAt);

        //    await _container.UpsertItemAsync(
        //        document,
        //        new PartitionKey(document.Id),
        //        cancellationToken: cancellationToken);
        //}
    }


}
