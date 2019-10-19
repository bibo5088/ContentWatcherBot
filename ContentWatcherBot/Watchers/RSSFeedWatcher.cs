using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
{
    /// <inheritdoc />
    class RssFeedWatcher : Watcher
    {
        public const string ListName = "rss_feed";
        public const string ListDescription = "Watch an RSS feed";

        private readonly Uri url;

        public RssFeedWatcher(string url)
        {
            this.url = new Uri(url);
            Name = $"rss_feed_{url}";
            Description = $"Watching RSS feed : ${url}";
            UpdateMessage = $"New content from ${this.url.Host}";
        }

        protected override Task<IDictionary<string, string>> FetchContent(HttpClient client)
        {
            throw new System.NotImplementedException();
        }
    }
}