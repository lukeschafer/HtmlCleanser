using System;
using System.Collections.Generic;
using System.Linq;
using HtmlAgilityPack;
using HtmlCleanser.Rules;

namespace HtmlCleanser
{
    public interface IHtmlCleanserRule
    {
        void Perform(HtmlDocument doc);
        string Perform(string htmlInput);
    }

    public interface IHtmlCleanser
    {
        string CleanseFull(string htmlInput, bool justReturnTheBodyContents = false);
        string PartialCleanse(string htmlInput, bool justReturnTheBodyContents = false, params Type[] rulesToIgnore);
        string MoveCssInline(string htmlInput, bool justReturnTheBodyContents = false);
    }

    public class HtmlCleanser : IHtmlCleanser
    {
        private readonly List<IHtmlCleanserRule> _rules;

        public HtmlCleanser(params IHtmlCleanserRule[] extraRules)
        {
            _rules = new List<IHtmlCleanserRule>
                {
                    new ConditionalCommentsShouldBeIgnored(),
                    new CommentsShouldBeRemoved(),
                    new XmlNamespacesShouldBeStripped(),
                    new StylesShouldBeInline(), // always first HTML Document cleanse rule
                    new DocumentShouldNotReferenceResources(),
                    new TextNodesShouldBeEscaped(),
                    new BaseTagShouldNotHaveHref(),
                };
            _rules.AddRange(extraRules);
        }

        public string CleanseFull(string htmlInput, bool justReturnTheBodyContents = false)
        {
            return Perform(htmlInput, _rules, new Type[] { }, justReturnTheBodyContents);
        }

        public string PartialCleanse(string htmlInput, bool justReturnTheBodyContents = false, params Type[] rulesToIgnore)
        {
            return Perform(htmlInput, _rules, rulesToIgnore, justReturnTheBodyContents);
        }

        public string MoveCssInline(string htmlInput, bool justReturnTheBodyContents = false)
        {
            return Perform(htmlInput, new IHtmlCleanserRule[] { new StylesShouldBeInline() }, new Type[] { }, justReturnTheBodyContents);
        }

        private static string Perform(string htmlInput, IEnumerable<IHtmlCleanserRule> rulesToUse, Type[] rulesToIgnore, bool justReturnTheBodyContents)
        {
            var rulesArray = rulesToUse.ToArray();
            rulesToIgnore = rulesToIgnore ?? new Type[] { };

            // First, parse the raw HTML to remove things that can't be cleansed as an XML document
            // ReSharper disable once LoopCanBeConvertedToQuery
            foreach (var rule in rulesArray.Where(r => !rulesToIgnore.Any(ignore => ignore.IsInstanceOfType(r))))
                htmlInput = rule.Perform(htmlInput);

            var doc = new HtmlDocument();
            doc.LoadHtml(htmlInput);

            // Second, cleanse the XML nodes themselves
            foreach (var rule in rulesArray.Where(r => !rulesToIgnore.Any(ignore => ignore.IsInstanceOfType(r)))) 
                rule.Perform(doc);

            var htmlText = justReturnTheBodyContents ?  doc.DocumentNode.SelectSingleNode("//body").InnerHtml : doc.DocumentNode.OuterHtml;
            return rulesArray.Any(r => r is TextNodesShouldBeEscaped) ? htmlText.Replace("<>", "") : htmlText; //sometimes we get an empty node thanks to some unencoded lt or gt
        }
    }
}