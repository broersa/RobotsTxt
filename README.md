# RobotsTxt #
A [robots.txt](http://en.wikipedia.org/wiki/Robots.txt) parser written in C# with .NET Core 2.0.

I did a fork of the original RobotsTxt package to build a version for the new dotnetcore framework.

>The Robot Exclusion Standard, also known as the Robots Exclusion Protocol or robots.txt protocol, is a  convention to prevent cooperating web spiders and other web robots from accessing all or part of a website which is otherwise publicly viewable. Robots are often used by search engines  to categorize and archive web sites, or by webmasters to proofread source code. The standard is unrelated to, but can be used in conjunction with, Sitemaps, a robot inclusion standard for websites.

## Supports ##
 * [Allow directives](http://en.wikipedia.org/wiki/Robots.txt#Allow_directive).
 * [Crawl-delay directives](http://en.wikipedia.org/wiki/Robots.txt#Crawl-delay_directive).
 * [Sitemap declarations](http://en.wikipedia.org/wiki/Robots.txt#Sitemap).
 * `*` and `$` wildcards. (See [here](http://www.bing.com/community/blogs/webmaster/archive/2008/06/03/robots-exclusion-protocol-joining-together-to-provide-better-documentation.aspx) and [here](http://www.google.com/support/webmasters/bin/answer.py?hl=en&answer=156449) for more information.)

###Usage ###

```
#!c#

Robots robots = Robots.Load(content); // Parses the robots.txt file's content.
// You can also use the constructor. (eg: Robots robots = new Robots(content);)

// To check if the file allows you to request a path :
bool canIGoThere = robots.IsPathAllowed(myUserAgent, path);

// CrawlDelay method will return the number of milliseconds to wait between successive requests
long crawlDelay = robots.CrawlDelay(myUserAgent); // Will return zero if there are no crawl-delay rules for this robot.

```

### Get ###
Install via [NuGet](https://www.nuget.org/packages/Com.Bekijkhet.RobotsTxt) :
```
PM> Install-Package Com.Bekijkhet.RobotsTxt
```