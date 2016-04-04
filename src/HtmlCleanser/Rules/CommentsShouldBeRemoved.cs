using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    public class CommentsShouldBeRemoved : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
        }

        public string Perform(string htmlInput)
        {
            var commentRegex = new Regex("<!--.*?-->", RegexOptions.Singleline);
            return commentRegex.Replace(htmlInput, "");
        }
    }
}