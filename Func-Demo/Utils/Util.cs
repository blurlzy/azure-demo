
using System.Text.RegularExpressions;

namespace Func_Demo.Utils
{
     internal static class Util
     {
          public static string ExtractFirstParagraph(string? html)
          {
               if (string.IsNullOrEmpty(html))
                    return string.Empty;

               var match = Regex.Match(html, @"<p[^>]*>.*?</p>", RegexOptions.Singleline | RegexOptions.IgnoreCase);
               return match.Success ? match.Value : string.Empty;
          }
     }
}
