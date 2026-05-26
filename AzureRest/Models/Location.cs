

namespace ConsoleApp1.Models
{
     // Minimal models for the response
     public sealed class LocationsResponse
     {
          public Location[]? Value { get; set; }
     }

     public sealed class Location
     {
          public string Name { get; set; } = "";
          public string DisplayName { get; set; } = "";
          public LocationMetadata? Metadata { get; set; }
     }

     public sealed class LocationMetadata
     {
          public string? RegionType { get; set; }   // Physical / Logical
          public string? Geography { get; set; }    // e.g. Asia Pacific
     }
}
