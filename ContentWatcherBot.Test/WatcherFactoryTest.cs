using System.Linq;
using System.Threading.Tasks;
using ContentWatcherBot.Test.MockResponses;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class WatcherFactoryTest
    {
        [Test]
        public async Task RssFeed()
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
            var watcher = WatcherFactory.CreateWatcher("rss_feed", "http://rss.com/feed");
            await watcher.FirstFetch();

            Assert.AreEqual("Mon site", watcher.Title);
            Assert.AreEqual("Ceci est un exemple de flux RSS 2.0", watcher.Description);

            //No new content
            Assert.IsEmpty(await watcher.NewContent());

            //New content
            var content = (await watcher.NewContent()).ToArray();
            Assert.AreEqual(1, content.Length);
            Assert.AreEqual("http://www.example.org/actu2", content[0]);
        }
    }
}