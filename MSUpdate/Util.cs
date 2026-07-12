using System.Text.RegularExpressions;

namespace MSUpdate
{
     internal static class Util
     {
          public static string ExtractFirstParagraph(string? html)
          {
               if (string.IsNullOrEmpty(html))
                    return null;

               var match = Regex.Match(html, @"<p[^>]*>.*?</p>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
               return match.Success ? match.Value : string.Empty;
          }
     }
}
