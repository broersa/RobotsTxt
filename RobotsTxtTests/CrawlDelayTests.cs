using System;
using Xunit;
using RobotsTxt;

namespace RobotsTxtTests
{
    public class CrawlDelayTests
    {
        private string nl = Environment.NewLine;

        [Theory]
        [InlineData("")]
        [InlineData(" ")]
        public void CrawlDelay_EmptyUserAgent_ThrowsArgumentException(
            string userAgent // white space considered empty
            )
        {
            Robots r = new Robots(String.Empty);
            Assert.Throws<ArgumentException>(() => r.CrawlDelay(userAgent));
        }

        [Fact]
        public void CrawlDelay_NoRules_Zero()
        {
            Robots r = new Robots(String.Empty);
            Assert.Equal(0, r.CrawlDelay("*"));
        }

        [Fact]
        public void CrawlDelay_NoCrawlDelayRule_Zero()
        {
            string s = @"User-agent: *" + nl + "Disallow: /dir/";
            Robots r = new Robots(s);
            Assert.Equal(0, r.CrawlDelay("*"));
        }

        [Fact]
        public void CrawlDelay_NoRuleForRobot_Zero()
        {
            string s = @"User-agent: Slurp" + nl + "Crawl-delay: 2";
            Robots r = new Robots(s);
            Assert.Equal(0, r.CrawlDelay("Google"));
        }

        [Fact]
        public void CrawlDelay_InvalidRule_Zero()
        {
            string s = @"User-agent: *" + nl + "Crawl-delay: foo";
            Robots r = new Robots(s);
            Assert.Equal(0, r.CrawlDelay("Google"));
        }

        [Fact]
        public void CrawlDelay_RuleWithoutUserAgent()
        {
            string s = "Crawl-delay: 1";
            Robots r = Robots.Load(s);
            Assert.NotEqual(1000, r.CrawlDelay("Google"));
            Assert.Equal(0, r.CrawlDelay("Google"));
        }

        [Theory]
        [InlineData(2000, "Google")]
        [InlineData(2000, "google")]
        [InlineData(500, "Slurp")]
        [InlineData(500, "slurp")]
        public void CrawlDelay_ValidRule(
            long expected,
            string userAgent)
        {
            string s = @"User-agent: Google" + nl + "Crawl-delay: 2" + nl +
                "User-agent: Slurp" + nl + "Crawl-delay: 0.5";
            Robots r = new Robots(s);
            Assert.Equal(expected, r.CrawlDelay(userAgent));
        }
    }
}