using ContentWatcherBot.Watchers;
using NUnit.Framework;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class WatcherFactoryTest
    {
        [TestCase("http://site.com", ExpectedResult = new[]
            {"rss_feed_http://site.com/", "Watch RSS feed http://site.com/", "New content from site.com"})]
        [TestCase("https://other-site.com/rss", ExpectedResult = new[]
        {
            "rss_feed_https://other-site.com/rss", "Watch RSS feed https://other-site.com/rss",
            "New content from other-site.com"
        })]
        public string[] RssFeed(string url)
        {
            var watcher = WatcherFactory.CreateWatcher("rss_feed", url);
            return new[] {watcher.Name, watcher.Description, watcher.UpdateMessage};
        }

        [Test]
        public void RssFeedInvalidUrl()
        {
            Assert.Throws<InvalidWatcherArgumentException>(() =>
            {
                WatcherFactory.CreateWatcher("rss_feed", "ftp://dgdq57ae");
            });
        }
    }
}