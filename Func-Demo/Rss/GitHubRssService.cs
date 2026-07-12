
using System.ServiceModel.Syndication;
using System.Xml;

namespace Func_Demo.Rss
{
     public sealed class GitHubRssService
     {
          private const string RssFeedUrl = "https://github.blog/changelog/feed/";
          private readonly HttpClient _httpClient;

          public GitHubRssService(HttpClient httpClient)
          {
               _httpClient = httpClient;
          }


          public async Task<IReadOnlyCollection<GitHubFeedItem>> GetLatestAsync(
              CancellationToken cancellationToken = default)
          {

               using var response = await _httpClient.GetAsync(RssFeedUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
               response.EnsureSuccessStatusCode();

               await using var stream = await response.Content.ReadAsStreamAsync(cancellationToken);

               using var xmlReader = XmlReader.Create(
                   stream,
                   new XmlReaderSettings
                   {
                        Async = true,
                        DtdProcessing = DtdProcessing.Prohibit,
                        XmlResolver = null
                   });

               SyndicationFeed feed;

               feed = SyndicationFeed.Load(xmlReader)
                       ?? throw new InvalidOperationException(
                           "The GitHub RSS feed did not contain a valid syndication feed.");

               return feed.Items
                   .Select(MapFeedItem)
                   .Where(item => item is not null)
                   .Cast<GitHubFeedItem>()
                   .OrderByDescending(item => item.PublishedAt)
                   .ToArray();
          }

          private static GitHubFeedItem? MapFeedItem(SyndicationItem item)
          {
               var link = item.Links
                   .FirstOrDefault(link => link.RelationshipType == "alternate")
                   ?.Uri
                   ?? item.Links.FirstOrDefault()?.Uri;

               if (link is null)
               {
                    return null;
               }

               var publishedAt = item.PublishDate != DateTimeOffset.MinValue
                   ? item.PublishDate
                   : item.LastUpdatedTime;

               return new GitHubFeedItem
               {
                    Id = string.IsNullOrWhiteSpace(item.Id)
                       ? link.ToString()
                       : item.Id,

                    Title = item.Title?.Text?.Trim() ?? "Untitled",

                    Summary = GetSummary(item),

                    Url = link,

                    PublishedAt = publishedAt,

                    Categories = item.Categories
                       .Select(category => category.Name)
                       .Where(name => !string.IsNullOrWhiteSpace(name))
                       .Distinct(StringComparer.OrdinalIgnoreCase)
                       .ToArray()
               };
          }

          private static string? GetSummary(SyndicationItem item)
          {
               var content = item.Summary?.Text;

               if (string.IsNullOrWhiteSpace(content) &&
                   item.Content is TextSyndicationContent textContent)
               {
                    content = textContent.Text;
               }

               return string.IsNullOrWhiteSpace(content)
                   ? null
                   : content.Trim();
          }
     }
}
