using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
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
        private static readonly Dictionary<string, WatcherCommand> WatcherCommands =
            new Dictionary<string, WatcherCommand>
            {
                //RssFeedWatcher
                [RssFeedWatcher.ListName] = new WatcherCommand
                {
                    Description = RssFeedWatcher.ListDescription,
                    HasArg = true,
                    ArgDescription = "Url of the RSS feed",
                    CreateWatcher = (url) => new RssFeedWatcher(url)
                }
            };

        public static async Task<Watcher> CreateWatcher(string name, string? arg)
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
            
            await watcher.FirstFetch();

            return watcher;
        }
    }
}