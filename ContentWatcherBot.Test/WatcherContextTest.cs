using System.Collections;
using System.Linq;
using System.Threading.Tasks;
using ContentWatcherBot.Database;
using ContentWatcherBot.Test.MockResponses;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class WatcherContextTest
    {
        private WatcherContext _context;

        [SetUp]
        public void SetUp()
        {
            var options = new DbContextOptionsBuilder<WatcherContext>().UseInMemoryDatabase("watcher").Options;

            _context = new WatcherContext(options);
            //Empty tables
            _context.Servers.RemoveRange(_context.Servers);
            _context.Watchers.RemoveRange(_context.Watchers);
            _context.ServerWatchers.RemoveRange(_context.ServerWatchers);

            _context.SaveChanges();
        }

        [Test]
        public async Task AddWatcher()
        {
            //Mock
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);
            Helpers.MockWatcherHttpClient(mockHttp);

            await _context.AddWatcher(WatcherFactory.CreateWatcher("rss_feed", "http://rss.com/feed"));

            Assert.AreEqual(1, _context.Watchers.Count());

            //Add the same
            await _context.AddWatcher(WatcherFactory.CreateWatcher("rss_feed", "http://rss.com/feed"));

            Assert.AreEqual(1, _context.Watchers.Count());
        }

        [Test]
        public async Task GetNewContentMessages()
        {
            //Watcher
            var mockHttp = new MockHttpMessageHandler();

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed1Xml.Value);

            mockHttp.Expect("http://rss.com/feed")
                .Respond("application/rss+xml", ExampleRssFeed2Xml.Value);
            Helpers.MockWatcherHttpClient(mockHttp);

            var watcher = await _context.AddWatcher(WatcherFactory.CreateWatcher("rss_feed", "http://rss.com/feed"));

            //Server
            var server = new Server {DiscordId = 123};
            _context.Servers.Add(server);

            //ServerWatcher
            var serverWatcher = new ServerWatcher {Server = server, Watcher = watcher, ChannelId = 123};
            _context.ServerWatchers.Add(serverWatcher);

            await _context.SaveChangesAsync();

            var messages = await _context.GetNewContentMessages();

            Assert.AreEqual(1, messages.Count);
            Assert.Contains(123, (ICollection) messages.Keys);
            Assert.AreEqual("http://www.example.org/actu2", messages[123][0]);
        }


        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}