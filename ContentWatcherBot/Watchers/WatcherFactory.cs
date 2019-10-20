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
                ["rss_feed"] = new WatcherCommand
                {
                    Description = "Watch an RSS feed",
                    HasArg = true,
                    ArgDescription = "Url of the RSS feed",
                    CreateWatcher = (arg) =>
                    {
                        if (!(Uri.TryCreate(arg, UriKind.Absolute, out var url)
                              && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps)))
                        {
                            throw new InvalidWatcherArgumentException($"`{arg}` is not a valid url");
                        }

                        var name = $"rss_feed_{url}";
                        var desc = $"Watch RSS feed {url}";
                        var updateMessage = $"New content from {url.Host}";
                        return new RssFeedWatcher(name, desc, updateMessage, url);
                    }
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