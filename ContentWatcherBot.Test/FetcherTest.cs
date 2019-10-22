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
            Helpers.MockWatcherHttpClient(mockHttp);

            //Fetcher
            var result = await Fetchers.RssFeedFetcher.FetchContent(new Uri("http://rss.com/feed"));

            //Title
            Assert.AreEqual("Mon site", result.Title);

            //Description
            Assert.AreEqual("Ceci est un exemple de flux RSS 2.0", result.Description);

            //Content
            Assert.AreEqual(1, result.Content.Count);
            Assert.Contains("Sat, 07 Sep 2002 00:00:01 GMT", result.Content.Keys);
            Assert.Contains("http://www.example.org/actu1", result.Content.Values);
        }
        
    }
}