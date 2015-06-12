HtmlCleanser [![Build status](https://ci.appveyor.com/api/projects/status/44eou144a3q6hj2b?svg=true)](https://ci.appveyor.com/project/lukeschafer/htmlcleanser)
============

HTML Cleaner and Pre-mailer for .Net.

Many email clients are strict in regards to what they allow. For example, GMail will not use stylesheets included in the document - only inline styles will be used. To see if your emails pass the test, run them through http://emaillint.com (https://github.com/lukeschafer/emaillint).

Not just for emails. Also good for ensuring clean, dependency free content from CMSs and the like.

This was created a long time ago as an alternative to Premailer.Net (which doesn't work properly!) to solve an actual business need. As of writing, it has been running in production, cleansing hundreds of emails per month, for the last few years. Just got around to open sourcing it :)

## CI + Nuget

CI: https://ci.appveyor.com/project/lukeschafer/htmlcleanser 

Nuget: https://www.nuget.org/packages/HtmlCleanser/

## API

```
  IHtmlCleanser cleanser = new HtmlCleanser()
  var clean = cleanser.CleanseFull(@"<html><body><base href='test'></body></html>");
  //"<html><body><base></body></html>"
```

### CleanseFull

```string CleanseFull(string htmlInput, bool justReturnTheBodyContents = false)```

Perform a full clean of an HTML document.

Args:
* [string] htmlInput - the HTML to fix
* [bool] justReturnTheBodyContents (default:true) - return just the contents of the body tag (no head etc)

What it does: 
* Removes Conditional Comments (leaving content)
* Inlines CSS from included stylesheets to the style tag of each node
* Removes External Resource References (stylesheets, javascript etc)
* Fixes unescaped text in nodes. This could actually cause problems, but it tries to fix them and will more likely fix things than break them (famous last words)
* Removes 'href' from \<base/\> tag

### MoveCssInline

```string MoveCssInline(string htmlInput, bool justReturnTheBodyContents = false)```

Inline CSS from included stylesheets to the style tag of each node. This also needs to fix conditional comments because they explode.

Args:
* [string] htmlInput - the HTML to inline
* [bool] justReturnTheBodyContents (default:true) - return just the contents of the body tag (no head etc)

What it does: 
* Removes Conditional Comments (leaving content)
* Inlines CSS from included stylesheets to the style tag of each node

### PartialCleanse

```string PartialCleanse(string htmlInput, bool justReturnTheBodyContents = false, params Type[] rulesToIgnore);```

Do a 'CleanseFull', but optionally ignore certain rules.

Args:
* [string] htmlInput - the HTML to inline
* [bool] justReturnTheBodyContents (default:true) - return just the contents of the body tag (no head etc)
* [Type[]] rulesToIgnore - the Type of any rules you don't want to run (e.g. TextNodesShouldBeEscaped if it's breaking something)

What it does: 
* Removes Conditional Comments (leaving content)
* Inlines CSS from included stylesheets to the style tag of each node
