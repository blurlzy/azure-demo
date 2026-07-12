using System.Globalization;
using System.Xml.Linq;

namespace MSUpdate
{

     public sealed class AzureUpdatesRssService
     {
          private const string RssFeedUrl = "https://www.microsoft.com/releasecommunications/api/v2/azure/rss";
          private static readonly XNamespace Atom = "http://www.w3.org/2005/Atom";

          private readonly HttpClient _httpClient;

          public AzureUpdatesRssService(HttpClient httpClient)
          {
               _httpClient = httpClient;
          }

          public async Task<IReadOnlyList<AzureUpdate>> GetUpdatesAsync(CancellationToken cancellationToken = default)
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

               return items ?? [];
          }

          private static AzureUpdate MapItem(XElement item)
          {
               var descriptionElement = item.Element("description");

               return new AzureUpdate(
                   Id: item.Element("guid")?.Value
                       ?? throw new InvalidOperationException("RSS item has no GUID."),
                   // IsPermaLink: ParseBoolean(item.Element("guid")?.Attribute("isPermaLink")?.Value),
                   Title: item.Element("title")?.Value ?? string.Empty,
                   Link: item.Element("link")?.Value ?? string.Empty,
                   Categories: item.Elements("category")
                       .Select(category => category.Value.Trim())
                       .Where(category => !string.IsNullOrWhiteSpace(category))
                       .ToArray(),
                   Description: descriptionElement?.Value.Trim() ?? string.Empty,
                   PublishedAt: ParseDate(item.Element("pubDate")?.Value),
                   UpdatedAt: ParseDate(item.Element(Atom + "updated")?.Value));
          }

          private static DateTimeOffset? ParseDate(string? value) =>
              DateTimeOffset.TryParse(
                  value,
                  CultureInfo.InvariantCulture,
                  DateTimeStyles.AssumeUniversal,
                  out var date)
                  ? date
                  : null;

          //private static bool? ParseBoolean(string? value) =>
          //    bool.TryParse(value, out var result) ? result : null;
     }


}
