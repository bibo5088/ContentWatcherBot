using System;
using System.Collections.Generic;
using ContentWatcherBot.Database;
using ContentWatcherBot.Fetcher;

namespace ContentWatcherBot
{
    public struct WatcherCommand
    {
        public string Description;

        public bool HasArg;

        public string? ArgDescription;

        public Func<string, Watcher> CreateWatcher;
    }

    public static class WatcherFactory
    {
        public static readonly Dictionary<string, WatcherCommand> WatcherCommands =
            new Dictionary<string, WatcherCommand>
            {
                ["rss_feed"] = new WatcherCommand
                {
                    Description = "Watch an RSS feed",
                    HasArg = true,
                    ArgDescription = "Url of the RSS feed",
                    CreateWatcher = (arg) => new Watcher(FetcherType.RssFeed, arg)
                }
            };

        public static Watcher CreateWatcher(string name, string? arg)
        {
            if (!WatcherCommands.TryGetValue(name, out var command))
            {
                throw new InvalidWatcherNameException();
            }

            if (command.HasArg && string.IsNullOrEmpty(arg))
            {
                throw new MissingWatcherArgumentException(command.ArgDescription);
            }

            var watcher = command.CreateWatcher(arg ?? "");

            return watcher;
        }
    }
}