
namespace Func_Demo.Rss
{
     // azure update rss item
     public sealed record AzureUpdate(
         string Id, // guid element value
         string Title,
         string Link,
         IReadOnlyList<string> Categories,
         string Description,
         DateTimeOffset? PublishedAt,
         DateTimeOffset? UpdatedAt);

     // ms foundry update rss item
     public sealed record FoundryUpdate(
          string Id, // guid element value
          string Title,
          string Link,
          //IReadOnlyList<string> Categories,
          string Description,
          string? Creator,
          DateTimeOffset? PublishedAt,
          DateTimeOffset? UpdatedAt);

     // fabric update rss item
     public sealed record FabricUpdate(
         string Id,
         string Title,
         string Link,
         string DescriptionHtml,
         string? Creator,
         DateTimeOffset? PublishedAt,
         DateTimeOffset? CreatedAt);

     // github rss item
     public sealed record GitHubFeedItem
     {
          public required string Id { get; init; }
          public required string Title { get; init; }
          public string? Summary { get; init; }
          public required Uri Url { get; init; }
          public DateTimeOffset PublishedAt { get; init; }
          public IReadOnlyCollection<string> Categories { get; init; } = [];
     }

     // create const ants for the sources
     public static class UpdateSources
     {
          public const string Azure = "Azure";
          public const string Foundry = "Microsoft Foundry";
          public const string Fabric = "Microsoft Fabric";
          public const string GitHub = "GitHub";
     }
}
