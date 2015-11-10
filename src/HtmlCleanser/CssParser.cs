using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace HtmlCleanser
{
    /// <summary>
    /// Parses CSS text into a dictionary of selector:StyleClass. Gratitude and credit to Hernan Garcia, adapted from https://gist.github.com/hgarcia/561823
    /// </summary>
    public class CssParser
    {
        private readonly List<KeyValuePair<string, StyleClass>> _parsedStyles;

        public CssParser()
        {
            _parsedStyles = new List<KeyValuePair<string, StyleClass>>();
        }

        public IEnumerable<KeyValuePair<string, StyleClass>> Styles
        {
            get { return _parsedStyles; }
        }

        /// <summary>
        /// Add a stylesheet's text content. This should be called in order. Assumes there are no media queries!
        /// </summary>
        /// <param name="styleSheetContent">CSS text (no tags)</param>
        public void AddStyleSheet(string styleSheetContent)
        {
            ProcessStyleSheet(styleSheetContent);
        }

        /// <summary>
        /// Parse a given style string, parse it and associate with a given selector. Useful for parsing style attributes
        /// </summary>
        /// <param name="selector">Selector to associate with</param>
        /// <param name="style">Style string</param>
        public StyleClass ParseStyleClass(string selector, string style)
        {
            return FillStyleClass(selector, style);
        }

        /// <summary>
        /// Parses a whole CSS stylesheet and processes each StyleClass. This assumes no media queries!
        /// </summary>
        /// <param name="styleSheetContent"></param>
        protected void ProcessStyleSheet(string styleSheetContent)
        {
            var content = CleanUp(styleSheetContent);
            var parts = content.Split('}');

            foreach (var s in parts.Where(s => CleanUp(s).IndexOf('{') > -1))
            {
                FillStyleClass(s);
            }
        }

        protected void FillStyleClass(string s)
        {
            var parts = s.Split('{');
            var selector = CleanUp(parts[0]).Trim();

            FillStyleClass(selector, parts[1]);
        }

        protected StyleClass FillStyleClass(string selector, string style)
        {
            var sc = new StyleClass { Selector = selector };

            var attrs = UnescapeNastyCharacters(CleanUp(style)).Split(';');

            foreach (var a in attrs)
            {
                if (!a.Contains(":")) continue;
                var key = a.Split(':')[0].Trim();

                if (sc.Attributes.ContainsKey(key))
                {
                    sc.Attributes.Remove(key);
                }

                var value = a.Split(':')[1].Trim().ToLower();
                sc.Attributes.Add(key, EscapeNastyCharacters(value));
            }

            _parsedStyles.Add(new KeyValuePair<string, StyleClass>(sc.Selector, sc));
            return sc;
        }

        /// <summary>
        /// Remove things like quotes
        /// </summary>
        protected static string EscapeNastyCharacters(string cssValue)
        {
            var noAmpersands = new Regex("&(?!(?:apos|quot|[gl]t|#[0-9]+|amp);)", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            return noAmpersands.Replace(cssValue, "&amp;").Replace("\"", "&quot;").Replace("'", "&#39;");
        }

        /// <summary>
        /// Remove things like quotes
        /// </summary>
        protected static string UnescapeNastyCharacters(string cssValue)
        {
            return cssValue
                .Replace("&amp;", "&")
                .Replace("&#38;", "&")
                .Replace("&quot;", "\"")
                .Replace("&#34;", "\"")
                .Replace("&#39;", "'")
                .Replace("&apos;", "'");
        }

        /// <summary>
        /// Cleans a selector, removing whitespace etc
        /// </summary>
        protected static string CleanUp(string s)
        {
            var temp = s;
            var r = new Regex("(/\\*(.|[\r\n])*?\\*/)|(//.*)"); //comments
            temp = r.Replace(temp, "");
            temp = temp.Replace("\r", "").Replace("\n", "");

            return temp;
        }
    }
}
