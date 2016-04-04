using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    public class XmlNamespacesShouldBeStripped : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
        }

        public string Perform(string htmlInput)
        {
            htmlInput = Regex.Replace(htmlInput, @"<\?xml.*?\?>", "");
            htmlInput = Regex.Replace(htmlInput, @"xmlns\:[\S]=""[^""]*""", "");
            htmlInput = Regex.Replace(htmlInput, @"<(/?)([\S]\:)([^>]*)>", "<$1$3>");

            return htmlInput;
        }
    }
}