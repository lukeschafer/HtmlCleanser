using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using HtmlAgilityPack;
using HtmlCleanser.Rules;

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
            var doc = new HtmlDocument();
            
            //we have to remove conditional comments because they totally break stuff. Leave the content
            var conditionalCommentRegex = new Regex("<!(--)?\\[[^]]*\\](--)?>");
            htmlInput = conditionalCommentRegex.Replace(htmlInput, "");

            var commentRegex = new Regex("<!--.*?-->", RegexOptions.Singleline);
            htmlInput = commentRegex.Replace(htmlInput, "");
            doc.LoadHtml(htmlInput);
            rulesToIgnore = rulesToIgnore ?? new Type[] {};

            foreach (var rule in rulesArray.Where(r => !rulesToIgnore.Any(ignore => ignore.IsInstanceOfType(r)))) 
                rule.Perform(doc);

            var htmlText = justReturnTheBodyContents ?  doc.DocumentNode.SelectSingleNode("//body").InnerHtml : doc.DocumentNode.OuterHtml;
            return rulesArray.Any(r => r is TextNodesShouldBeEscaped) ? htmlText.Replace("<>", "") : htmlText; //sometimes we get an empty node thanks to some unencoded lt or gt
        }
    }
}