using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
{
    /// <inheritdoc />
    class RSSFeedWatcher : Watcher
    {
        protected override Task<IDictionary<string, string>> FetchContent(HttpClient client)
        {
            throw new System.NotImplementedException();
        }
    }
}