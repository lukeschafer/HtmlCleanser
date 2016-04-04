using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    /// <summary>
    /// Unescaped characters in text nodes can cause errors and display quirks. Indeed, this rule could 
    /// cause display issues if the parser has trouble, but it's likely to output something sane.
    /// </summary>
    public class TextNodesShouldBeEscaped : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//text()");
            if (nodes == null) return;
            foreach (var n in nodes)
                n.InnerHtml = System.Net.WebUtility.HtmlEncode(System.Net.WebUtility.HtmlDecode(n.InnerHtml));
        }

        public string Perform(string htmlInput)
        {
            return htmlInput;
        }
    }
}
