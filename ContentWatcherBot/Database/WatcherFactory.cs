using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentWatcherBot.Database.Watchers;
using ContentWatcherBot.Fetcher;
using Discord;

namespace ContentWatcherBot.Database
{
    public static class WatcherFactory
    {
        public static async Task<Watcher> CreateWatcher(Uri url)
        {
            //Mangadex
            var mangadexWatcher = await CreateMangadexWatcher(url);
            if (mangadexWatcher.IsSpecified) return mangadexWatcher.Value;

            //ItchIo
            var itchIoWatcher = await CreateItchIoWatcher(url);
            if (itchIoWatcher.IsSpecified) return itchIoWatcher.Value;

            //RssFeed
            var rssWatcher = await CreateRssWatcher(url);
            if (rssWatcher.IsSpecified) return rssWatcher.Value;

            //Error
            throw new UnknownWatcherUrl(url);
        }

        private static readonly Regex MangadexUrlRegex = new Regex(
            @"^https?:\/\/mangadex\.(?:org|com)\/title\/(\d+)\/?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static async Task<Optional<Watcher>> CreateMangadexWatcher(Uri url)
        {
            try
            {
                //Regex Url
                var match = MangadexUrlRegex.Match(url.ToString());

                //Url doesn't match
                var mangaIdGroup = match.Groups[1];
                if (!match.Success || !mangaIdGroup.Success) return Optional<Watcher>.Unspecified;

                var mangaId = mangaIdGroup.Value;
                var result = await Fetchers.MangadexFetcher.FetchContent(mangaId);
                return new Watcher(FetcherType.Mangadex, url, mangaId, result.Title, result.Description,
                    new HashSet<string>(result.Content.Keys));
            }
            catch
            {
                return Optional<Watcher>.Unspecified;
            }
        }

        private static readonly Regex ItchIoUrlRegex = new Regex(@"^https?:\/\/\w+\.itch.io\/\w+?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static async Task<Optional<Watcher>> CreateItchIoWatcher(Uri uri)
        {
            try
            {
                var url = uri.ToString();

                //Regex Url
                var match = ItchIoUrlRegex.Match(url);

                //Url doesn't match
                if (!match.Success) return Optional<Watcher>.Unspecified;

                var result = await Fetchers.ItchIoFetcher.FetchContent(url);
                return new Watcher(FetcherType.ItchIo, uri, url, result.Title, result.Description,
                    new HashSet<string>(result.Content.Keys));
            }
            catch
            {
                return Optional<Watcher>.Unspecified;
            }
        }

        private static async Task<Optional<Watcher>> CreateRssWatcher(Uri url)
        {
            try
            {
                var param = url.ToString();
                var result = await Fetchers.RssFeedFetcher.FetchContent(param);
                return new Watcher(FetcherType.RssFeed, url, param, result.Title, result.Description,
                    new HashSet<string>(result.Content.Keys));
            }
            catch
            {
                return Optional<Watcher>.Unspecified;
            }
        }
    }
}