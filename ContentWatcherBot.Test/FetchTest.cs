using System;
using System.Threading.Tasks;
using ContentWatcherBot.Database.Watchers;
using ContentWatcherBot.Test.MockResponses;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class FetchTest
    {
        [Test]
        public async Task RssFeedFetcher()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);
            Helpers.MockWatcherHttpClient(mockHttp);

            //Fetch
            var watcher = new RssFeedWatcher(new Uri("http://rss.com/feed"));
            var result = await watcher.Fetch();

            //Title
            Assert.AreEqual("Mon site", result.Title);

            //Description
            Assert.AreEqual("Ceci est un exemple de flux RSS 2.0", result.Description);

            //Content
            Assert.AreEqual(1, result.Content.Count);
            Assert.Contains("Sat, 07 Sep 2002 00:00:01 GMT", result.Content.Keys);
            Assert.Contains("http://www.example.org/actu1", result.Content.Values);
        }

        [Test]
        public async Task MangadexFetcher()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect("https://mangadex.org/api/manga/123")
                .Respond("application/json", MangadexManga1.Value);
            Helpers.MockWatcherHttpClient(mockHttp);

            //Fetch
            var watcher = new MangadexWatcher("123");
            var result = await watcher.Fetch();

            //Title
            Assert.AreEqual("Beast Complex", result.Title);

            //Description
            Assert.AreEqual(
                "A collection of short stories that involve anthropomorphic animals and their troubles with coexisting with different species. Itagaki Paru's debut manga.",
                result.Description);

            //Content
            Assert.AreEqual(2, result.Content.Count);
            Assert.Contains("132633", result.Content.Keys);
            Assert.Contains("https://mangadex.org/chapter/132636", result.Content.Values);
        }

        [Test]
        public async Task ItchIoFetcher()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect("https://terrycavanagh.itch.io/dicey-dungeons")
                .Respond("text/html", ItchIo1.Value);
            Helpers.MockWatcherHttpClient(mockHttp);

            //Fetch
            var watcher = new ItchIoWatcher(new Uri("https://terrycavanagh.itch.io/dicey-dungeons"));
            var result = await watcher.Fetch();

            //Title
            Assert.AreEqual("Dicey Dungeons", result.Title);

            //Description
            Assert.AreEqual(
                "Become a giant walking dice and battle to the end of an ever-changing dungeon! Can you escape the cruel whims of Lady Luck?",
                result.Description);

            //Content
            Assert.AreEqual(8, result.Content.Count);
            Assert.Contains("28 October 2019 @ 18:24", result.Content.Keys);
            Assert.Contains("https://terrycavanagh.itch.io/dicey-dungeons/devlog/106966/halloween-special",
                result.Content.Values);
        }
    }
}