using System;
using Xunit;
using RobotsTxt;

namespace RobotsTxtTests
{
    public class SimpleTests
    {
        private string nl = Environment.NewLine;

        [Fact]
        public void Robots_DefaultAllowDirectiveImplementation_MoreSpecific()
        {
            Robots r = new Robots(String.Empty);
            var actual = r.AllowRuleImplementation;
            var expected = AllowRuleImplementation.MoreSpecific;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void Robots_DirectiveWithoutUserAgent_Malformed()
        {
            string s = "Disallow: /file.html";
            Robots r = new Robots(s);
            Assert.True(r.Malformed);
        }

        [Fact]
        public void Robots_InvalidUserAgent_Malformed()
        {
            string s = "User-agent: " + nl + "Disallow: /file.html";
            Robots r = new Robots(s);
            Assert.True(r.Malformed);
            Assert.True(r.IsPathAllowed("myRobot", "/file.html"));
        }

        [Fact]
        public void Robots_InvalidLine_Malformed()
        {
            string s = "User-agent: *" + nl + "foo";
            Robots r = new Robots(s);
            Assert.True(r.Malformed);
        }

        [Fact]
        public void Robots_WithAbsoluteSitemapDirective_HasSitemapUrlAndValue()
        {
            string s = "User-agent: *" + nl + "Sitemap: http://foo.com/sitemap.xml";
            Robots r = new Robots(s);
            Assert.Equal(1, r.Sitemaps.Count);
        }

        [Fact]
        public void Robots_WithRelativeSitemapDirective_OnlyHasSitemapValue()
        {
            string s = "User-agent: *" + nl + "Sitemap: /sitemap.xml";
            Robots r = new Robots(s);
            Assert.Equal(1, r.Sitemaps.Count);
        }

        [Fact]
        public void Robots_MultipleSitemapsWithoutUserAgent()
        {
            string s = "Sitemap: http://foo.com/sitemap.xml" + nl +
                "Sitemap: http://foo.com/sitemap2.xml" + nl +
                "Sitemap: http://foo.com/sitemap3.xml";
            Robots r = new Robots(s);
            Assert.False(r.Malformed);
            Assert.Equal(3, r.Sitemaps.Count);
        }

        [Fact]
        public void Robots_Sitemap_Various()
        {
            string s = "User-agent: *" + nl + "Sitemap: http://foo.com/sitemap.xml";
            Robots r = new Robots(s);
            Assert.Equal(new Uri("http://foo.com/sitemap.xml"), r.Sitemaps[0].Url);
            Assert.Equal("http://foo.com/sitemap.xml", r.Sitemaps[0].Value);
            s = "User-agent: *" + nl + "Sitemap: /sitemap.xml";
            r = new Robots(s);
            Assert.Equal(new Uri("file:///sitemap.xml"), r.Sitemaps[0].Url);
            s = "User-agent: *" + nl + "Sitemap: /sitemap.xml" + nl + "Sitemap: http://foo.com/sitemap.xml";
            r = new Robots(s);
            Assert.Equal(2, r.Sitemaps.Count);
        }

        [Fact]
        public void Robots_CommentsShouldBeIgnored()
        {
            string s = "User-agent: *" + nl + "#Disallow: /";
            Robots r = new Robots(s);
            Assert.False(r.HasRules);
            s = "User-agent: *" + nl + "Disallow: /";
            r = new Robots(s);
            Assert.True(r.HasRules);
        }

        [Fact]
        public void Robots_ValidWithRules_NotMalformedAndHasRules()
        {
            string s = "User-agent: *" + nl + "Disallow: /";
            Robots r = new Robots(s);
            Assert.False(r.Malformed);
            Assert.True(r.HasRules);
        }

        [Fact]
        public void Robots_WhiteSpaceOnlyLines_StripsOutWhiteSpaceOnlyLines()
        {
            string s = "User-agent: *" + nl + "    " + nl + "Disallow: /";
            Robots r = new Robots(s);
            Assert.False(r.Malformed);
            Assert.True(r.HasRules);
        }

        [Fact]
        public void Robots_ValidWithoutRules_NotMalformedAndDoesntHaveRules()
        {
            string s = "User-agent: *";
            Robots r = new Robots(s);
            Assert.False(r.Malformed);
            Assert.False(r.HasRules);
        }

        [Fact]
        public void Robots_InValidWithRules_NotMalformedAndDoesntHaveRules()
        {
            string s = "User-agent: *" + nl + "Disallow: /" + nl + "foo";
            Robots r = new Robots(s);
            Assert.True(r.Malformed);
            Assert.True(r.HasRules);
        }

        [Fact]
        public void Robots_InvalidWithoutRules_MalformedAndDoesntHAveRules()
        {
            string s = "User-agent: *" + nl + "foo: bar";
            Robots r = new Robots(s);
            Assert.True(r.Malformed);
            Assert.False(r.HasRules);
        }

        [Fact]
        public void Robots_IsAnyPathDisallowed()
        {
            string s = "User-agent: *" + nl + "Crawl-delay: 5";
            Robots r = new Robots(s);
            Assert.False(r.IsAnyPathDisallowed);
            s = "User-agent: *" + nl + "Disallow: ";
            r = new Robots(s);
            Assert.False(r.IsAnyPathDisallowed);
            s = "User-agent: *" + nl + "Disallow: /";
            r = new Robots(s);
            Assert.True(r.IsAnyPathDisallowed);
            s = "User-agent: *" + nl + "Disallow: /file.html";
            r = new Robots(s);
            Assert.True(r.IsAnyPathDisallowed);
        }
    }
}