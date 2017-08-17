using System;
using Xunit;
using RobotsTxt;

namespace RobotsTxtTests
{
    public class IsPathAllowedTests
    {
        private string nl = Environment.NewLine;

        [Theory]
        [InlineData("", "")]
        [InlineData(" ", "")]
        public void IsPathAllowed_EmptyUserAgent_ThrowsArgumentException(
            string userAgent, // white space considered empty
            string path)
        {
            string s = "User-agent: *" + nl + "Disallow: /";
            Robots r = new Robots(s);
            Assert.Throws<ArgumentException>(() => r.IsPathAllowed(userAgent, path));
        }

        [Fact]
        public void IsPathAllowed_RuleWithoutUserAgent_True()
        {
            string s = "Disallow: /";
            Robots r = Robots.Load(s);
            Assert.True(r.IsPathAllowed("*", "/foo"));
        }

        [Theory]
        [InlineData("*", "")]
        [InlineData("*", "/")]
        [InlineData("*", "/file.html")]
        [InlineData("*", "/directory/")]
        [InlineData("some robot", "")]
        [InlineData("some robot", "/")]
        [InlineData("some robot", "/file.html")]
        [InlineData("some robot", "/directory/")]
        public void IsPathAllowed_WithoutRules_True(
            string userAgent,
            string path)
        {
            Robots r = new Robots(String.Empty);
            Assert.True(r.IsPathAllowed(userAgent, path));
        }

        [Theory]
        [InlineData("*", "")]
        [InlineData("*", "/")]
        [InlineData("*", "/file.html")]
        [InlineData("*", "/directory/")]
        [InlineData("some robot", "")]
        [InlineData("some robot", "/")]
        [InlineData("some robot", "/file.html")]
        [InlineData("some robot", "/directory/")]
        public void IsPathAllowed_WithoutAccessRule_True(
            string userAgent,
            string path)
        {
            string s = "User-agent: *" + nl + "Crawl-delay: 5";
            Robots r = new Robots(s);
            Assert.True(r.IsPathAllowed(userAgent, path));
        }

        [Theory]
        [InlineData("")]
        [InlineData("/")]
        [InlineData("/file.html")]
        [InlineData("/directory/")]
        public void IsPathAllowed_NoRulesForRobot_True(
            string path)
        {
            string s = "User-agent: Slurp" + nl + "Disallow: /";
            Robots r = new Robots(s);
            Assert.True(r.IsPathAllowed("some robot", path));
        }

        [Theory]
        [InlineData("Slurp", "")]
        [InlineData("Slurp", "/")]
        [InlineData("Slurp", "/file.html")]
        [InlineData("Slurp", "/directory/")]
        [InlineData("slurp", "")]
        [InlineData("slurp", "/")]
        [InlineData("slurp", "/file.html")]
        [InlineData("slurp", "/directory/")]
        [InlineData("Exabot", "")]
        [InlineData("Exabot", "/")]
        [InlineData("Exabot", "/file.html")]
        [InlineData("Exabot", "/directory/")]
        [InlineData("exabot", "")]
        [InlineData("exabot", "/")]
        [InlineData("exabot", "/file.html")]
        [InlineData("exabot", "/directory/")]
        public void IsPathAllowed_NoGlobalRules_False(
            string userAgent,
            string path)
        {
            string s = "User-agent: Slurp" + nl + "Disallow: /" + nl + "User-agent: Exabot" + nl + "Disallow: /";
            Robots r = new Robots(s);
            Assert.False(r.IsPathAllowed(userAgent, path));
        }

        [Theory]
        [InlineData("Slurp")]
        [InlineData("slurp")]
        [InlineData("Exabot")]
        [InlineData("exabot")]
        [InlineData("FigTree/0.1 Robot libwww-perl/5.04")]
        public void IsPathAllowed_UserAgentStringCaseInsensitive_False(
            string userAgent)
        {
            string s = 
@"User-agent: Slurp
Disallow: /
User-agent: Exabot
Disallow: /
User-agent: Exabot
Disallow: /
User-agent: figtree
Disallow: /";
            Robots r = Robots.Load(s);
            Assert.False(r.IsPathAllowed(userAgent, "/dir"));
        }

        [Theory]
        [InlineData("/help")]
        [InlineData("/help.ext")]
        [InlineData("/help/")]
        [InlineData("/help/file.ext")]
        [InlineData("/help/dir/")]
        [InlineData("/help/dir/file.ext")]
        public void IsPathAllowed_OnlyDisallow_False(
            string path)
        {
            string s = @"User-agent: *" + nl + "Disallow: /help";
            Robots r = new Robots(s);
            Assert.False(r.IsPathAllowed("*", path));
        }

        [Theory]
        [InlineData("foo")]
        [InlineData("/dir/file.ext")]
        [InlineData("/dir/file.ext1")]
        public void IsPathAllowed_AllowAndDisallow_True(
            string path)
        {
            string s = @"User-agent: *" + nl + "Allow: /dir/file.ext" + nl + "Disallow: /dir/";
            Robots r = new Robots(s);
            Assert.True(r.IsPathAllowed("*", path));
        }

        [Theory]
        [InlineData("/dir/file2.ext")]
        [InlineData("/dir/")]
        [InlineData("/dir/dir/")]
        public void IsPathAllowed_AllowAndDisallow_False(
            string path)
        {
            string s = @"User-agent: *" + nl + "Allow: /dir/file.ext" + nl + "Disallow: /dir/";
            Robots r = new Robots(s);
            Assert.False(r.IsPathAllowed("*", path));
        }

        [Theory]
        [InlineData("/dir/file.ext", "/dir/File.ext")]
        [InlineData("/dir/file.ext", "/Dir/file.ext")]
        [InlineData("/dir/file.ext", "/a/File.html")]
        [InlineData("/dir/file.ext", "a.GIF")]
        [InlineData("/dir/file.ext", "/dir/File.ext")]
        [InlineData("/dir/file.ext", "/Dir/file.ext")]
        [InlineData("/dir/file.ext", "/a/File.html")]
        [InlineData("/dir/file.ext", "a.GIF")]
        [InlineData("/*/file.html", "/dir/File.ext")]
        [InlineData("/*/file.html", "/Dir/file.ext")]
        [InlineData("/*/file.html", "/a/File.html")]
        [InlineData("/*/file.html", "a.GIF")]
        [InlineData("/*.gif$", "/dir/File.ext")]
        [InlineData("/*.gif$", "/Dir/file.ext")]
        [InlineData("/*.gif$", "/a/File.html")]
        [InlineData("/*.gif$", "a.GIF")]
        public void IsPathAllowed_PathShouldBeCaseSensitive_True(
            string rule,
            string path)
        {
            string s = @"User-agent: *" + nl + "Disallow: " + rule;
            Robots r = Robots.Load(s);
            Assert.True(r.IsPathAllowed("*", path));
        }

        [Theory]
        [InlineData("asd")]
        [InlineData("a.gifa")]
        [InlineData("a.gif$")]
        public void IsPathAllowed_DollarWildcard_True(
            string path)
        {
            string s = @"User-agent: *" + nl + "Disallow: /*.gif$";
            Robots r = Robots.Load(s);
            Assert.True(r.IsPathAllowed("*", path));
        }

        [Theory]
        [InlineData("a.gif")]
        [InlineData("foo.gif")]
        [InlineData("b.a.gif")]
        [InlineData("a.gif.gif")]
        public void IsPathAllowed_DollarWildcard_False(
            string path)
        {
            string s = @"User-agent: *" + nl + "Disallow: /*.gif$";
            Robots r = Robots.Load(s);
            Assert.False(r.IsPathAllowed("*", path));
        }

        [Theory]
        [InlineData("/*/file.html", "/foo/", true)]
        [InlineData("/*/file.html", "file.html", true)]
        [InlineData("/*/file.html", "/foo/file2.html", true)]
        [InlineData("/*/file.html", "/a/file.html", false)]
        [InlineData("/*/file.html", "/dir/file.html", false)]
        [InlineData("/*/file.html", "//a//file.html", false)]
        [InlineData("/*/file.html", "/a/a/file.html", false)]
        [InlineData("/*/file.html", "/a/a/file.htmlz", false)]
        [InlineData("/*/file.html", "///f.html", true)]
        [InlineData("/*/file.html", "/\\/f.html", true)]
        [InlineData("/*/file.html", "/:/f.html", true)]
        [InlineData("/*/file.html", "/*/f.html", true)]
        [InlineData("/*/file.html", "/?/f.html", true)]
        [InlineData("/*/file.html", "/\"/f.html", true)]
        [InlineData("/*/file.html", "/</f.html", true)]
        [InlineData("/*/file.html", "/>/f.html", true)]
        [InlineData("/*/file.html", "/|/f.html", true)]
        [InlineData("/private*/", "/private/", false)]
        [InlineData("/private*/", "/Private/", true)]
        [InlineData("/private*/", "/private/f.html", false)]
        [InlineData("/private*/", "/private/dir/", false)]
        [InlineData("/private*/", "/private/dir/f.html", false)]
        [InlineData("/private*/", "/private1/", false)]
        [InlineData("/private*/", "/Private1/", true)]
        [InlineData("/private*/", "/private1/f.html", false)]
        [InlineData("/private*/", "/private1/dir/", false)]
        [InlineData("/private*/", "/private1/dir/f.html", false)]
        [InlineData("*/private/", "/private/dir/f.html", false)]
        public void IsPathAllowed_StarWildcard(string rule, string path, Boolean result)
        {
            string s = @"User-agent: *" + nl + "Disallow: " + rule;
            Robots r = Robots.Load(s);
            Assert.Equal(result, r.IsPathAllowed("*", path));
        }
     }
}
