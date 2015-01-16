using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    /// <summary>
    /// Remove 'href' attribute from 'base' tags. There should only be 0..1, but this handles 0..N. 
    /// An href in base will change the behaviour of all links in the document.
    /// </summary>
    public class BaseTagShouldNotHaveHref : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//base");
            if (nodes == null) return;
            foreach (var n in nodes) n.Attributes.Remove("href");
            //TODO: should we match relative URLs in <a /> and prepend the (first) base URL?
        }
    }
}
