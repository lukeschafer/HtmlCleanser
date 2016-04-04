using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    /// <summary>
    /// Cleansed documents should not try to use external resources, or have inline script tags.
    /// </summary>
    public class DocumentShouldNotReferenceResources : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//script|//link");
            if (nodes == null) return;
            foreach (var resource in nodes) resource.Remove();
        }

        public string Perform(string htmlInput)
        {
            return htmlInput;
        }
    }
}
