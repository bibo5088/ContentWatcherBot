using System;

namespace ContentWatcherBot.Database.Watchers
{
    public abstract class UrlWatcher : Watcher
    {
        public Uri Url { get; private set; }

        public UrlWatcher()
        {
        }

        public UrlWatcher(Uri url)
        {
            Url = url;
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ Url.GetHashCode();
        }
    }
}