using NUnit.Framework;

namespace HtmlCleanser.Tests
{
    [TestFixture]
    public class HtmlCleanserTests
    {
        private const string MsOfficeHtml = "<html xmlns:v=\"urn:schemas-microsoft-com:vml\" xmlns:o=\"urn:schemas-microsoft-com:office:office\" xmlns:w=\"urn:schemas-microsoft-com:office:word\" xmlns:m=\"http://schemas.microsoft.com/office/2004/12/omml\" xmlns=\"http://www.w3.org/TR/REC-html40\">\r\n<head>\r\n<meta http-equiv=\"Content-Type\" content=\"text/html; charset=iso-8859-1\">\r\n<meta name=\"Generator\" content=\"Microsoft Word 14 (filtered medium)\">\r\n<style><!--\r\n/* Font Definitions */\r\n@font-face\r\n\t{font-family:Calibri;\r\n\tpanose-1:2 15 5 2 2 2 4 3 2 4;}\r\n@font-face\r\n\t{font-family:Tahoma;\r\n\tpanose-1:2 11 6 4 3 5 4 4 2 4;}\r\n/* Style Definitions */\r\np.MsoNormal, li.MsoNormal, div.MsoNormal\r\n\t{margin:0cm;\r\n\tmargin-bottom:.0001pt;\r\n\tfont-size:11.0pt;\r\n\tfont-family:\"Calibri\",\"sans-serif\";\r\n\tmso-fareast-language:EN-US;}\r\na:link, span.MsoHyperlink\r\n\t{mso-style-priority:99;\r\n\tcolor:blue;\r\n\ttext-decoration:underline;}\r\na:visited, span.MsoHyperlinkFollowed\r\n\t{mso-style-priority:99;\r\n\tcolor:purple;\r\n\ttext-decoration:underline;}\r\np.MsoAcetate, li.MsoAcetate, div.MsoAcetate\r\n\t{mso-style-priority:99;\r\n\tmso-style-link:\"Balloon Text Char\";\r\n\tmargin:0cm;\r\n\tmargin-bottom:.0001pt;\r\n\tfont-size:8.0pt;\r\n\tfont-family:\"Tahoma\",\"sans-serif\";\r\n\tmso-fareast-language:EN-US;}\r\nspan.EmailStyle17\r\n\t{mso-style-type:personal-compose;\r\n\tfont-family:\"Calibri\",\"sans-serif\";\r\n\tcolor:windowtext;}\r\nspan.BalloonTextChar\r\n\t{mso-style-name:\"Balloon Text Char\";\r\n\tmso-style-priority:99;\r\n\tmso-style-link:\"Balloon Text\";\r\n\tfont-family:\"Tahoma\",\"sans-serif\";}\r\n.MsoChpDefault\r\n\t{mso-style-type:export-only;\r\n\tfont-family:\"Calibri\",\"sans-serif\";\r\n\tmso-fareast-language:EN-US;}\r\n@page WordSection1\r\n\t{size:612.0pt 792.0pt;\r\n\tmargin:72.0pt 72.0pt 72.0pt 72.0pt;}\r\ndiv.WordSection1\r\n\t{page:WordSection1;}\r\n--></style><!--[if gte mso 9]><xml>\r\n<o:shapedefaults v:ext=\"edit\" spidmax=\"1026\" />\r\n</xml><![endif]--><!--[if gte mso 9]><xml>\r\n<o:shapelayout v:ext=\"edit\">\r\n<o:idmap v:ext=\"edit\" data=\"1\" />\r\n</o:shapelayout></xml><![endif]-->\r\n</head>\r\n<body lang=\"EN-AU\" link=\"blue\" vlink=\"purple\">\r\n<div class=\"WordSection1\">\r\n<p class=\"MsoNormal\">MY MESSAGE<o:p></o:p></p>\r\n<p class=\"MsoNormal\"><o:p>&nbsp;</o:p></p>\r\n<p class=\"MsoNormal\"><o:p>&nbsp;</o:p></p>\r\n</div>\r\n</body>\r\n</html>";
        #region CleanseFull

        [Test]
        public void CleanseFullShouldRemoveResources()
        {
            const string html = @"<html><head><script>alert('hi');</script><script src='//test.com/js.js'></script><link href='test1' /></head><body><link href='test2' /><script>alert('hi');</script>asd</body></html>";
            var cleansed = new HtmlCleanser().CleanseFull(html);
            Assert.AreEqual("<html><head></head><body>asd</body></html>", cleansed);
        }

        [Test]
        public void CleanseFullShouldRemoveBaseHref()
        {
            const string html = @"<html><body><base href='test'></body></html>";
            var cleansed = new HtmlCleanser().CleanseFull(html);
            Assert.AreEqual("<html><body><base></body></html>", cleansed);
        }

        [Test]
        public void CleanseFullShouldInlineStylesAndRemoveTag()
        {
            const string html = @"<html><body><style>p{color:red;}.test{float:left;}</style><p class='test'>asd</p></body></html>";
            var cleansed = new HtmlCleanser().CleanseFull(html);
            Assert.AreEqual("<html><body><p class='test' style=\"float: left;color: red;\">asd</p></body></html>", cleansed);
        }

        [Test]
        public void CleanseFullShouldNotDieForBaseTagWithNoHref()
        {
            const string html = @"<html><body><base></body></html>";
            var cleansed = new HtmlCleanser().CleanseFull(html);
            Assert.AreEqual("<html><body><base></body></html>", cleansed);
        }

        [Test]
        public void CleanseFullShouldEscapeCrappyChars()
        {
            const string html = @"<html><body>asd<asd<div><</div>&</body></html>";
            var cleansed = new HtmlCleanser().CleanseFull(html);
            Assert.AreEqual("<html><body>asd<div></div>&amp;</body></html>", cleansed);
        }

        [Test]
        public void CleanseFullShouldBeAbleToReturnJustBody()
        {
            const string html = @"<html><body><div>asdf</div></body></html>";
            var cleansed = new HtmlCleanser().CleanseFull(html, true);
            Assert.AreEqual("<div>asdf</div>", cleansed);
        }

        [Test]
        public void CleanseFullShouldHandleCrazyWordHtml()
        {
            var cleansed = new HtmlCleanser().CleanseFull(MsOfficeHtml, true);
            Assert.True(cleansed.Contains("MY MESSAGE"));//just assert pretty much that there were no exceptions
        }

        [Test]
        public void CleanseFullShouldStripNamespacesFromCrazyWordHtml()
        {
            var cleansed = new HtmlCleanser().CleanseFull(MsOfficeHtml, true);
            Assert.False(cleansed.Contains("<o:"));
            Assert.False(cleansed.Contains("xmlns"));
        }

        #endregion

        #region MoveCssInline

        [Test]
        public void MoveCssInlineShouldBeAbleToReturnJustBody()
        {
            const string html = @"<html><body><div>asdf</div></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html, true);
            Assert.AreEqual("<div>asdf</div>", cleansed);
        }

        [Test]
        public void MoveCssInlineShouldInlineStylesAndRemoveTag()
        {
            const string html = @"<html><body><style>p{color:red;}.test{float:left;}</style><p class='test'>asd</p></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual("<html><body><p class='test' style=\"float: left;color: red;\">asd</p></body></html>", cleansed);
        }
        [Test]
        public void MoveCssInlineShouldntDoAnythingElseExceptMinorFormatting()
        {
            const string html = @"<html><head><script>alert('hi');</script><link href='test1'></head><body><base href='test'>a&sd</body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual(html, cleansed);
        }
        [Test]
        public void MoveCssInlineIgnoresBadMediaQueries()
        {
            const string html = @"<html><head><style>p{color:red}@media print {p{color:black;}}</style></head><body><p>a</p></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual("<html><head></head><body><p style=\"color: red;\">a</p></body></html>", cleansed);
        }
        [Test]
        public void MoveCssInlineAppliesMediaQueryForAll()
        {
            const string html = @"<html><head><style>p{color:red}@media all {p{color:black;}}</style></head><body><p>a</p></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual("<html><head></head><body><p style=\"color: black;\">a</p></body></html>", cleansed);
        }
        [Test]
        public void MoveCssInlineAppliesMediaQueryForScreen()
        {
            const string html = @"<html><head><style>p{color:red}@media screen {p{color:black;}}</style></head><body><p>a</p></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual("<html><head></head><body><p style=\"color: black;\">a</p></body></html>", cleansed);
        }
        [Test]
        public void MoveCssInlineShouldHandleDuplicateSelectorsInSameStylesheet()
        {
            const string html = @"<html><head><style>p.test{color:red;height:12px} p.test{color:black;}</style></head><body><p class='test'>a</p></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual("<html><head></head><body><p class='test' style=\"color: black;height: 12px;\">a</p></body></html>", cleansed);
        }
        [Test]
        public void MoveCssInlineShouldEncodeQuotes()
        {
            const string html = @"<html><head><style>div { font-family: ""Times&New'Roman&amp;""; }</style></head><body><div>Foo</div></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual(@"<html><head></head><body><div style=""font-family: &quot;times&amp;new&#39;roman&amp;&quot;;"">Foo</div></body></html>", cleansed);
        }
        [Test]
        public void MoveCssInlineShouldHandleEncodedStyleAttribute()
        {
            const string html = @"<html><head><style>div { font-family: arial; }</style></head><body><div style=""content:'&amp;'"">Foo</div></body></html>";
            var cleansed = new HtmlCleanser().MoveCssInline(html);
            Assert.AreEqual(@"<html><head></head><body><div style=""content: &#39;&amp;&#39;;font-family: arial;"">Foo</div></body></html>", cleansed);
        }

        #endregion

    }
}
