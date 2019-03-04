using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlCleanser.Rules;
using System.Reflection;

namespace HtmlCleanser
{
    public interface IHtmlCleanserRule
    {
        void Perform(HtmlDocument doc);
    }

    public interface IHtmlCleanser
    {
        string CleanseFull(string htmlInput, bool justReturnTheBodyContents = false);
        string PartialCleanse(string htmlInput, bool justReturnTheBodyContents = false, params Type[] rulesToIgnore);
        string MoveCssInline(string htmlInput, bool justReturnTheBodyContents = false);
        string Perform(string htmlInput, IEnumerable<IHtmlCleanserRule> rulesToUse, bool justReturnTheBodyContents);
    }

    public class HtmlCleanser : IHtmlCleanser
    {
        private readonly List<IHtmlCleanserRule> _rules; 
        public HtmlCleanser(params IHtmlCleanserRule[] extraRules)
        {
            _rules = new List<IHtmlCleanserRule>
                {
                    new StylesShouldBeInline(), // always first!
                    new DocumentShouldNotReferenceResources(),
                    new TextNodesShouldBeEscaped(),
                    new BaseTagShouldNotHaveHref(),
                    new StylesAttributesShouldHaveValue()
                };
            _rules.AddRange(extraRules);
        }
        public string CleanseFull(string htmlInput, bool justReturnTheBodyContents = false)
        {
            return Perform(htmlInput, _rules, justReturnTheBodyContents);
        }

        public string PartialCleanse(string htmlInput, bool justReturnTheBodyContents = false, params Type[] rulesToIgnore)
        {
            var ruleSet = _rules.Where(r => !rulesToIgnore.Any(ignore => ignore.GetTypeInfo().IsInstanceOfType(r)));
            return Perform(htmlInput, ruleSet, justReturnTheBodyContents);
        }

        public string MoveCssInline(string htmlInput, bool justReturnTheBodyContents = false)
        {
            return Perform(htmlInput, new IHtmlCleanserRule[] { new StylesShouldBeInline() }, justReturnTheBodyContents);
        }

        public string Perform(string htmlInput, IEnumerable<IHtmlCleanserRule> rulesToUse, bool justReturnTheBodyContents)
        {
            var rulesArray = rulesToUse.ToArray();
            var doc = new HtmlDocument();
            
            //we have to remove conditional comments because they totally break stuff. Leave the content
            var conditionalCommentRegex = new Regex("<!(--)?\\[[^]]*\\](--)?>");
            htmlInput = conditionalCommentRegex.Replace(htmlInput, "");

            var commentRegex = new Regex("<!--.*?-->", RegexOptions.Singleline);
            htmlInput = commentRegex.Replace(htmlInput, "");
            doc.LoadHtml(htmlInput);

            foreach (var rule in rulesArray) 
                rule.Perform(doc);

            var htmlText = justReturnTheBodyContents ?  doc.DocumentNode.SelectSingleNode("//body").InnerHtml : doc.DocumentNode.OuterHtml;
            return rulesArray.Any(r => r is TextNodesShouldBeEscaped) ? htmlText.Replace("<>", "") : htmlText; //sometimes we get an empty node thanks to some unencoded lt or gt
        }
    }
}