using System;
using System.Text.RegularExpressions;

namespace ContentWatcherBot.Database.Watchers
{
    public static class WatcherFactory
    {
        public static Watcher PickWatcher(Uri url)
        {
            //Mangadex
            if (PickMangadexWatcher(url, out var mangadexWatcher)) return mangadexWatcher;

            //ItchIo
            if (PickItchIoWatcher(url, out var itchIoWatcher)) return itchIoWatcher;

            //Default RssFeed
            return new RssFeedWatcher(url);
        }

        private static readonly Regex MangadexUrlRegex = new Regex(
            @"^https?:\/\/mangadex\.(?:org|com)\/title\/(\d+)\/?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static bool PickMangadexWatcher(Uri url, out Watcher watcher)
        {
            //Regex Url
            var match = MangadexUrlRegex.Match(url.ToString());

            //Url doesn't match
            var mangaIdGroup = match.Groups[1];
            if (!match.Success || !mangaIdGroup.Success)
            {
                watcher = null;
                return false;
            }

            var mangaId = mangaIdGroup.Value;

            watcher = new MangadexWatcher(mangaId);
            return true;
        }

        private static readonly Regex ItchIoUrlRegex = new Regex(@"^https?:\/\/\w+\.itch.io\/\w+?",
            RegexOptions.Compiled | RegexOptions.IgnoreCase);

        private static bool PickItchIoWatcher(Uri url, out Watcher watcher)
        {
            //Regex Url
            var match = ItchIoUrlRegex.Match(url.ToString());

            //Url doesn't match
            if (!match.Success)
            {
                watcher = null;
                return false;
            }

            watcher = new ItchIoWatcher(url);
            return true;
        }
    }
}