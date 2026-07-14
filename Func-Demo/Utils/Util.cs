
using System.Net;
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

           public static string ExtractFirstSentence(string? html)
           {
                if (string.IsNullOrWhiteSpace(html))
                     return string.Empty;

                var blockMatch = Regex.Match(
                     html,
                     @"<(?<tag>p|div)\b[^>]*>(?<content>.*?)</\k<tag>\s*>",
                     RegexOptions.Singleline | RegexOptions.IgnoreCase);
                var content = blockMatch.Success ? blockMatch.Groups["content"].Value : html;
                var text = Regex.Replace(content, @"<[^>]+>", " ");
                text = WebUtility.HtmlDecode(Regex.Replace(text, @"\s+", " ")).Trim();

                var sentenceMatch = Regex.Match(text, @"^.*?[.!?](?:[""'”’\)\]]+)?(?=\s|$)");
                return sentenceMatch.Success ? sentenceMatch.Value : text;
           }


     }
}
