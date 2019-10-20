using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using ContentWatcherBot.Test.MockResponses;
using ContentWatcherBot.Watchers;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class WatcherListTest
    {
        private WatcherList _watcherList;

        [SetUp]
        public void SetUp()
        {
            _watcherList = new WatcherList();
        }

        [Test]
        public async Task CheckAndGetMessages()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed2Xml.Value);

            Helpers.MockWatcherHttpClient(mockHttp);

            //Watcher
            var watcherName = await _watcherList.AddWatcher("rss_feed", "http://rss.com/feed");

            var messages = await _watcherList.CheckAndGetMessages();

            Assert.AreEqual(1, messages.Count);
            Assert.That(messages.ContainsKey(watcherName));
            Assert.AreEqual("New content from rss.com\nhttp://www.example.org/actu2", messages[watcherName][0]);
        }

        [Test]
        public void KeepOnly()
        {
            var watcherDict = (Dictionary<string, Watcher>)typeof(WatcherList).GetField("_watchers", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(_watcherList);
            watcherDict["1"] = null;
            watcherDict["2"] = null;
            watcherDict["3"] = null;
            
            Assert.AreEqual(3, _watcherList.Count);
            Assert.IsTrue(watcherDict.ContainsKey("2"));
            
            _watcherList.KeepOnly(new HashSet<string>{"1", "3"});
            
            Assert.AreEqual(2, _watcherList.Count);
            Assert.IsFalse(watcherDict.ContainsKey("2"));
        }
    }
}