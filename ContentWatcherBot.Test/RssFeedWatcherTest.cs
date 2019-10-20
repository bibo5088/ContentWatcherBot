using System.Linq;
using System.Threading.Tasks;
using ContentWatcherBot.Test.MockResponses;
using ContentWatcherBot.Watchers;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class RssFeedWatcherTest
    {
        [Test]
        public async Task Test()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed2Xml.Value);

            Helpers.MockWatcherHttpClient(mockHttp);

            //Watcher
            var watcher = new RssFeedWatcher("http://rss.com/feed");
            await watcher.FirstFetch();

            //No message
            Assert.IsEmpty(await watcher.CheckAndGetMessages());

            //One message
            var messages = await watcher.CheckAndGetMessages();
            Assert.AreEqual("New content from rss.com\nhttp://www.example.org/actu2", messages.First());
        }

        [Test]
        public void InvalidUrl()
        {
            Assert.Throws<InvalidWatcherArgumentException>(() =>
            {
                new RssFeedWatcher("ftp://r dqdq 4 dqss@m/fe74eaéé%ed");
            });
        }

        [Test]
        public void InvalidRssFeed()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.Expect("http://not-rss.com/feed")
                .Respond("application/json", @"{message:""hello world""}");

            var e = Assert.ThrowsAsync<FetchFailedException>(async () =>
            {
                var watcher = new RssFeedWatcher("http://not-rss.com/feed");
                await watcher.FirstFetch();
            });

            Assert.AreEqual("Failed to fetch content from http://not-rss.com/feed", e.Message);
        }
    }
}