using System.Collections.Generic;
using HtmlAgilityPack;

namespace HtmlCleanser.Rules
{
    /// <summary>
    /// Remove all styles attributtes that don`t have a value. For ex. style="font-family:Calibri,Arial,Helvetica,sans-serif; font-size:; margin: 0"
    /// => style="font-family:Calibri,Arial,Helvetica,sans-serif;margin: 0"
    /// </summary>
    public class StylesAttributesShouldHaveValue : IHtmlCleanserRule
    {
        public void Perform(HtmlDocument doc)
        {
            var nodes = doc.DocumentNode.Descendants();
            if (nodes == null) return;
            var cssParser = GetCssParser();
            foreach (var n in nodes)
            {
                var styleAttribute = n.Attributes["style"];
                if (styleAttribute != null)
                {
                    var parsedStyles = new StyleClass(cssParser.ParseStyleClass("styleTag", styleAttribute.Value));
                    var parsedStyleToDelete = new Dictionary<string, string>();
                    foreach (var a in parsedStyles.Attributes)
                    {
                        if (string.IsNullOrWhiteSpace(a.Value))
                            parsedStyleToDelete.Add(a.Key, a.Value);
                    }
                    if (parsedStyleToDelete.Count > 0)
                    {
                        foreach (var s in parsedStyleToDelete)
                            parsedStyles.Attributes.Remove(s.Key);
                        styleAttribute.Value = parsedStyles.ToString();
                    }
                }
            }
        }

        protected virtual CssParser GetCssParser()
        {
            return new CssParser();
        }
    }
}