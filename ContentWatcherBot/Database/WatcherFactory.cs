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
                var param = url.ToString();
                var result = await Fetchers.RssFeedFetcher.FetchContent(param);
                return new Watcher(FetcherType.RssFeed, url.ToHttp(), param, result.Title, result.Description,
                    new HashSet<string>(result.Content.Keys));
            }
            catch
            {
                return null;
            }
        }
    }
}