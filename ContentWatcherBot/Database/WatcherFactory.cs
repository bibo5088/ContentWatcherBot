using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ContentWatcherBot.Fetcher;
using Discord;

namespace ContentWatcherBot.Database
{
    public static class WatcherFactory
    {
        public static async Task<Watcher> CreateWatcher(Uri url)
        {
            //RssWatcher
            var rssWatcher = await CreateRssWatcher(url);
            if (rssWatcher.IsSpecified) return rssWatcher.Value;

            //Error
            throw new UnknownWatcherUrl(url);
        }

        private static async Task<Optional<Watcher>> CreateRssWatcher(Uri url)
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
                return Optional<Watcher>.Unspecified;
            }
        }
    }
}