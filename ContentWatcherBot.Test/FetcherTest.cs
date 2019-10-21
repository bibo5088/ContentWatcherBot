using System;
using System.Threading.Tasks;
using ContentWatcherBot.Fetcher;
using ContentWatcherBot.Test.MockResponses;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class FetcherTest
    {
        [Test]
        public async Task RssFeedFetcher()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);

            //Fetcher
            var fetcher = new RssFeedFetcher(new Uri("http://rss.com/feed"));
            var result = await fetcher.FetchContent(mockHttp.ToHttpClient());

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
        public void RssFeedFetcherInvalidUrl()
        {
            //Invalid url test
            var e = Assert.Throws<InvalidWatcherArgumentException>(() =>
                FetcherFactory.CreateFetcher(FetcherType.RssFeed, "ftp: dqs9dq \"dqq54&&&"));

            Assert.AreEqual("`ftp: dqs9dq \"dqq54&&&` is not a valid url", e.Message);
        }

        [Test]
        public void RssFeedFetcherInvalidFeed()
        {
            var mockHttp = new MockHttpMessageHandler();
            mockHttp.Expect("not-a-feed.com")
                .Respond("application/json", @"{""hello"":""world""");

            var e = Assert.ThrowsAsync<FetchFailedException>(async () =>
            {
                var fetcher = FetcherFactory.CreateFetcher(FetcherType.RssFeed, "http://not-a-feed.com");
                await fetcher.FetchContent(mockHttp.ToHttpClient());
            });

            Assert.AreEqual("Unable to fetch content from http://not-a-feed.com/", e.Message);
        }
    }
}