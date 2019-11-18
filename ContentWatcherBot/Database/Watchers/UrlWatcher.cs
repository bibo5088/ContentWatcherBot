using System;

namespace ContentWatcherBot.Database.Watchers
{
    public abstract class UrlWatcher : Watcher
    {
        public Uri Url { get; }

        public UrlWatcher(Uri url)
        {
            Url = url;
        }
    }
}