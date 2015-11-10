using System.Collections.Generic;
using System.Linq;

namespace HtmlCleanser
{
    public class StyleClass
    {
        public StyleClass()
        {
            Attributes = new Dictionary<string, string>();
        }

        public StyleClass(StyleClass sc)
        {
            Attributes = sc.Attributes.ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            Selector = sc.Selector;
        }

        public string Selector { get; set; }
        public Dictionary<string, string> Attributes { get; set; }

        /// <summary>
        /// For this StyleClass, merge in a StyleClass with a higher precedence
        /// </summary>
        /// <param name="moreRelevantStyleClass">The more relevant StyleClass</param>
        /// <param name="canOverwrite">Indicates whether styles of the same selector should override or be ignored</param>
        public void Merge(StyleClass moreRelevantStyleClass, bool canOverwrite)
        {
            foreach (var item in moreRelevantStyleClass.Attributes)
            {
                if (!Attributes.ContainsKey(item.Key))
                {
                    Attributes.Add(item.Key, item.Value);
                    continue;
                }
                if (item.Value.Contains("!important"))
                {
                    Attributes[item.Key] = item.Value;
                    continue;
                }
                if (canOverwrite)
                {
                    if (Attributes[item.Key].Contains("!important") && !item.Value.Contains("!important")) continue;
                    Attributes[item.Key] = item.Value;
                }
            }
        }

        /// <summary>
        /// For this StyleClass, merge in a StyleClass with a lower precedence. Conflicting styles of the merged class are ignored
        /// </summary>
        /// <param name="lessRelevantStyleClass">The less relevant StyleClass</param>
        public void ReverseMerge(StyleClass lessRelevantStyleClass)
        {
            foreach (var item in lessRelevantStyleClass.Attributes)
            {
                if (!Attributes.ContainsKey(item.Key))
                {
                    Attributes.Add(item.Key, item.Value);
                    continue;
                }
                if (item.Value.Contains("!important") && !Attributes[item.Key].Contains("!important"))
                {
                    Attributes[item.Key] = item.Value;
                }
            }
        }

        public override string ToString()
        {
            var parts = Attributes
                .Where(a => !string.IsNullOrWhiteSpace(a.Value))
                .Select(item => string.Format("{0}: {1};", item.Key, item.Value));
            return string.Join("", parts);
        }
    }
}
