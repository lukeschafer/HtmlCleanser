using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlCleanser.Rules
{
    /// <summary>
    /// Moves all stylesheets to inline styles of nodes matching the selectors. The Stylesheet is then removed. 
    /// This does not work on linked resources, just inline stylesheets.
    /// </summary>
    public class StylesShouldBeInline : IHtmlCleanserRule
    {
        private readonly Regex _mediaregex = new Regex("[@]media\\s+([^{ ]+)\\s*[{]", RegexOptions.IgnoreCase | RegexOptions.Compiled);

        public void Perform(HtmlDocument doc)
        {
            var styles = doc.DocumentNode.SelectNodes("//style") ?? new HtmlNodeCollection(doc.DocumentNode);
            foreach (var style in styles.Reverse())
            {
                if (style.Attributes["id"] != null && !string.IsNullOrWhiteSpace(style.Attributes["id"].Value) && style.Attributes["id"].Value.Equals("mobile", StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                var cssParser = GetCssParser();
                var cssBlock = style.InnerHtml;

                cssParser.AddStyleSheet(PrepCss(cssBlock));

                foreach (var item in cssParser.Styles.Reverse())
                {
                    var styleClassCss = new StyleClass(item.Value);

                    foreach (var element in doc.DocumentNode.QuerySelectorAll(styleClassCss.Selector))
                    {
                        var styleAttribute = element.Attributes["style"];
                        if (styleAttribute == null)
                        {
                            element.Attributes.Add("style", String.Empty);
                            styleAttribute = element.Attributes["style"];
                        }

                        var parsedStyles = new StyleClass(cssParser.ParseStyleClass("styleTag", styleAttribute.Value));
                        parsedStyles.ReverseMerge(new StyleClass(item.Value));

                        styleAttribute.Value = parsedStyles.ToString();
                    }
                }

                style.Remove();
            }
        }

        protected virtual CssParser GetCssParser()
        {
            return new CssParser();
        }

        /// <summary>
        /// Will remove @media queries, inlining 'screen' and 'all'
        /// </summary>
        protected string PrepCss(string css)
        {
            Match match;
            while ((match = _mediaregex.Match(css)).Success)
            {
                var mediaType = match.Groups.Count > 1 ? match.Groups[1].Value.ToLower() : "";
                var openCount = 0;
                int i;
                for (i = match.Index + match.Length; i < css.Length; i++)
                {
                    if (css[i] == '{') openCount++;
                    if (css[i] == '}') openCount--;
                    if (openCount == -1) break;
                }
                if (mediaType == "screen" || mediaType == "all")
                {
                    css = css.Remove(i, 1).Remove(match.Index, match.Length);
                }
                else
                {
                    css = css.Remove(match.Index, i - match.Index);
                }
            }

            return css;
        }
    }
}
