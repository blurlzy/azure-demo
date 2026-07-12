
using Microsoft.Identity.Client;
using System.Text.Json.Serialization;

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

     // cosmos db item model
     public sealed record UpdateItem(
          [property: JsonPropertyName("id")] string Id,
          // rss item id
          string RssItemId,
          string Source,
          string Title,
          string Link,
          IReadOnlyList<string> Categories,
          string Description,
          DateTimeOffset? PublishedAt,
          DateTimeOffset? UpdatedAt);
}
