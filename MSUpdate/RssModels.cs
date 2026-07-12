
namespace MSUpdate
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

     //public sealed record UpdateItem(
     //     string Id,
     //     // rss item id
     //     string RssItemId,
     //     string Source,
     //     string Title,
     //     string Link,
     //     IReadOnlyList<string>? Categories,
     //     string Description,
     //     string? Creator,
     //     DateTimeOffset? PublishedAt,
     //     DateTimeOffset? UpdatedAt);

     // create const ants for the sources
     public static class UpdateSources
     {
          public const string Azure = "Azure";
          public const string Foundry = "Microsoft Foundry";
          public const string Fabric = "Microsoft Fabric";
          public const string Github = "GitHub";
     }
}
