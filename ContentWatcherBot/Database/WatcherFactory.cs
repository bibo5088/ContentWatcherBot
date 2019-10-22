using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentWatcherBot.Fetcher;

namespace ContentWatcherBot.Database
{
    public static class WatcherFactory
    {
        public static async Task<Watcher> CreateWatcher(Uri url)
        {
            //RssWatcher
            var rssWatcher = await CreateRssWatcher(url);
            if (rssWatcher != null) return rssWatcher;

            //Error
            throw new UnknownWatcherUrl(url);
        }

        private static async Task<Watcher?> CreateRssWatcher(Uri url)
        {
            try
            {
                var result = await Fetchers.RssFeedFetcher.FetchContent(url);
                return new Watcher(FetcherType.RssFeed, url, result.Title, result.Description,
                    new HashSet<string>(result.Content.Keys));
            }
            catch
            {
                return null;
            }
        }
    }
}