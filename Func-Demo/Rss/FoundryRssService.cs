using System.Globalization;
using System.Xml.Linq;

namespace Func_Demo.Rss
{
     public sealed class FoundryRssService
     {
          private const string RssFeedUrl = "https://techcommunity.microsoft.com/t5/s/gxcuf89792/rss/board?board.id=azure-ai-foundry-blog";
          private static readonly XNamespace DC = "http://purl.org/dc/elements/1.1/";

          private readonly HttpClient _httpClient;

          // ctor
          public FoundryRssService(HttpClient httpClient)
          {
               _httpClient = httpClient;
          }

          public async Task<IReadOnlyList<FoundryUpdate>> GetUpdatesAsync(CancellationToken cancellationToken = default)
          {
               using var response = await _httpClient.GetAsync(RssFeedUrl, cancellationToken);
               response.EnsureSuccessStatusCode();

               await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);
               var document = await XDocument.LoadAsync(
                   stream,
                   LoadOptions.PreserveWhitespace,
                   cancellationToken);

               var items = document.Root?
                  .Element("channel")?
                   .Elements("item")
                   .Select(MapItem)
                   .ToArray();

               return items ?? Array.Empty<FoundryUpdate>();
          }

          private static FoundryUpdate MapItem(XElement item)
          {
               var descriptionElement = item.Element("description");

               return new FoundryUpdate(
                   Id: item.Element("guid")?.Value
                       ?? throw new InvalidOperationException("RSS item has no GUID."),
                   // IsPermaLink: ParseBoolean(item.Element("guid")?.Attribute("isPermaLink")?.Value),
                   Title: item.Element("title")?.Value ?? string.Empty,
                   Link: item.Element("link")?.Value ?? string.Empty,
                   //Categories: item.Elements("category")
                   //    .Select(category => category.Value.Trim())
                   //    .Where(category => !string.IsNullOrWhiteSpace(category))
                   //    .ToArray(),
                   Description: descriptionElement?.Value.Trim() ?? string.Empty,
                   Creator: item.Element(DC + "creator")?.Value,
                   PublishedAt: ParseDate(item.Element("pubDate")?.Value),
                   UpdatedAt: ParseDate(item.Element(DC + "date")?.Value));
          }

          private static DateTimeOffset? ParseDate(string? value) =>
              DateTimeOffset.TryParse(
                  value,
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.AssumeUniversal,
                  out var date)
                  ? date
                  : null;
     }
}
