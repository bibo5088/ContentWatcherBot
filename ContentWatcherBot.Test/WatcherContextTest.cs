using System;
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
            _context.Guilds.RemoveRange(_context.Guilds);
            _context.Watchers.RemoveRange(_context.Watchers);
            _context.GuildWatchers.RemoveRange(_context.GuildWatchers);

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

            await _context.AddWatcher(new Uri("http://rss.com/feed"));

            Assert.AreEqual(1, _context.Watchers.Count());

            //Add the same
            await _context.AddWatcher(new Uri("http://rss.com/feed"));

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

            var watcher = await _context.AddWatcher(new Uri("http://rss.com/feed"));

            //Guild
            var guild = new Guild {GuildId = 123};
            _context.Guilds.Add(guild);

            //ServerWatcher
            await _context.AddGuildWatcher(guild, watcher, 123);

            await _context.SaveChangesAsync();

            var messages = await _context.GetNewContentMessages();

            Assert.AreEqual(1, messages.Count);
            Assert.Contains(123, (ICollection) messages.Keys);
            Assert.AreEqual("New content from \"Mon site\"\nhttp://www.example.org/actu2", messages[123][0]);
        }
        
        [TearDown]
        public void TearDown()
        {
            _context.Dispose();
        }
    }
}