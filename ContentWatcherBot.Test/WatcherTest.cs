using System;
using System.Linq;
using System.Threading.Tasks;
using ContentWatcherBot.Database;
using ContentWatcherBot.Database.Watchers;
using ContentWatcherBot.Test.MockResponses;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class WatcherTest
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
            var watcher = await WatcherFactory.CreateWatcher(new Uri("http://rss.com/feed"));

            Assert.AreEqual("Mon site", watcher.Title);
            Assert.AreEqual("Ceci est un exemple de flux RSS 2.0", watcher.Description);

            //No new content
            Assert.IsEmpty(await watcher.NewContent());

            //New content
            var content = (await watcher.NewContent()).ToArray();
            Assert.AreEqual(1, content.Length);
            Assert.AreEqual("http://www.example.org/actu2", content[0]);
        }

        [Test]
        public async Task Mangadex()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.Expect("https://mangadex.org/api/manga/123")
                .Respond("application/json", MangadexManga1.Value);

            mockHttp.Expect("https://mangadex.org/api/manga/123")
                .Respond("application/json", MangadexManga1.Value);

            mockHttp.Expect("https://mangadex.org/api/manga/123")
                .Respond("application/json", MangadexManga2.Value);

            Helpers.MockWatcherHttpClient(mockHttp);

            //Watcher
            var watcher = await WatcherFactory.CreateWatcher(new Uri("https://mangadex.org/title/123/"));

            Assert.AreEqual("Beast Complex", watcher.Title);
            Assert.AreEqual(
                "A collection of short stories that involve anthropomorphic animals and their troubles with coexisting with different species. Itagaki Paru's debut manga.",
                watcher.Description);

            //No new content
            Assert.IsEmpty(await watcher.NewContent());

            //New content
            var content = (await watcher.NewContent()).ToArray();
            Assert.AreEqual(1, content.Length);
            Assert.AreEqual("https://mangadex.org/chapter/12345", content[0]);
        }
    }
}