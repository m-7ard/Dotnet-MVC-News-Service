using System.Text.RegularExpressions;
using System.Web;

namespace MVC_News.MVC.Services;

public class MarkupParser
{
    public static string ParseToHtml(string? input)
    {
        if (string.IsNullOrEmpty(input))
            return string.Empty;

        string processed = HttpUtility.HtmlEncode(input);

        processed = Regex.Replace(processed, @"\[bold\](.*?)\[/bold\]", "<strong>$1</strong>", RegexOptions.Singleline);
        processed = Regex.Replace(processed, @"\[italic\](.*?)\[/italic\]", "<em>$1</em>", RegexOptions.Singleline);
        processed = Regex.Replace(processed, @"\[paragraph\](.*?)\[/paragraph\]", "<p>$1</p>", RegexOptions.Singleline);
        processed = Regex.Replace(processed, @"\[subheader\](.*?)\[/subheader\]", "<h2>$1</h2>", RegexOptions.Singleline);

        processed = Regex.Replace(processed,
            @"\[image url=([^ ]+) caption=([^\]]+)\]",
            "<figure><img src=\"$1\" alt=\"$2\" /><figcaption>$2</figcaption></figure>");

        processed = Regex.Replace(processed,
            @"\[link url=([^\]]+)\](.*?)\[/link\]",
            "<a href=\"$1\">$2</a>");

        return processed;
    }
}