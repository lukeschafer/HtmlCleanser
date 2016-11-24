using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    public class RemoveTeletypeTextTags : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.SelectNodes("//tt");
            if (nodes == null) return;
            foreach (var node in nodes)
            {
                foreach (HtmlNode child in node.ChildNodes)
                {
                    node.ParentNode.InsertBefore(child, node);
                    node.Remove();
                }
            }
        }
    }
}
