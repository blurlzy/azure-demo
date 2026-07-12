
using System.Text.Json.Serialization;

namespace MSUpdate
{

     // cosmos db item model
     public class CosmosItem
     {
          public string Id { get; set; }

          [JsonIgnore]
          public string Partition => this.Id;

          public string RssItemId { get; set; }

          public string Source { get; set; }
          
          public string Link { get; set; }
          
          public string Title { get; set; }
          
          public string Description { get; set; }
          
          public string[] Categories { get; set; }
          
          public string? Creator { get; set; }
          
          public DateTimeOffset? PublishedAt { get; set; }
          
          public DateTimeOffset? UpdatedAt { get; set; }

          // ctor
          public CosmosItem(string rssItemId, 
                            string source, 
                            string link, 
                            string title, 
                            string desc, 
                            string[] categories, 
                            string? creator, 
                            DateTimeOffset? publishedAt, 
                            DateTimeOffset? updatedAt) 
          {

               this.Id = Guid.NewGuid().ToString();
               this.RssItemId = rssItemId;
               this.Source = source;
               this.Link = link;
               this.Title = title;
               this.Description = desc;
               this.Categories = categories;
               this.Creator = creator;
               this.PublishedAt = publishedAt;
               this.UpdatedAt = updatedAt;
          }
     }

}
