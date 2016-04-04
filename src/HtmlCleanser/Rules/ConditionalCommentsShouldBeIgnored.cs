using System.Text.RegularExpressions;
using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    public class ConditionalCommentsShouldBeIgnored : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
        }

        public string Perform(string htmlInput)
        {
            //we have to remove conditional comments because they totally break stuff. Leave the content
            var conditionalCommentRegex = new Regex("<!(--)?\\[[^]]*\\](--)?>");
            return conditionalCommentRegex.Replace(htmlInput, "");
        }
    }
}