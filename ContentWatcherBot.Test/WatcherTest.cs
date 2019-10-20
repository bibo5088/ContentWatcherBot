using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ContentWatcherBot.Watchers;
using NUnit.Framework;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class WatcherTest
    {
        private class TestWatcher : Watcher
        {
            public int State = 0;

            public TestWatcher()
            {
                UpdateMessage = "Test";
            }

            protected async override Task<IDictionary<string, string>> FetchContent(HttpClient client)
            {
                switch (State)
                {
                    case 0:
                        State++;
                        return new Dictionary<string, string>
                        {
                            ["1"] = "1"
                        };

                    case 1:
                        State++;
                        return new Dictionary<string, string>
                        {
                            ["1"] = "1"
                        };

                    default:
                        return new Dictionary<string, string>
                        {
                            ["1"] = "1",
                            ["2"] = "2",
                        };
                }
            }
        }


        [Test]
        public async Task Test()
        {
            var watcher = new TestWatcher();
            await watcher.FirstFetch();
            
            //No message
            Assert.IsEmpty(await watcher.CheckAndGetMessages());
       
            //One message
            var messages = await watcher.CheckAndGetMessages();
            Assert.AreEqual("Test\n2", messages.First());
        }
    }
}