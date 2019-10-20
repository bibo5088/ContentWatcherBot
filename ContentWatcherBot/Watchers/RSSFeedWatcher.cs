using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace ContentWatcherBot.Watchers
{
    /// <inheritdoc />
    public class RssFeedWatcher : Watcher
    {
        public const string ListName = "rss_feed";
        public const string ListDescription = "Watch an RSS feed";

        private readonly Uri _url;

        public RssFeedWatcher(string url)
        {
            if (!(Uri.TryCreate(url, UriKind.Absolute, out _url)
                && (_url.Scheme == Uri.UriSchemeHttp || _url.Scheme == Uri.UriSchemeHttps)))
            {
                throw new InvalidWatcherArgumentException($"`{url}` is not a valid url");
            }


            Name = $"rss_feed_{url}";
            Description = $"Watching RSS feed : {url}";
            UpdateMessage = $"New content from {_url.Host}";
        }

        public RssFeedWatcher(string name, string description, string updateMessage, string url)
        {
            _url = new Uri(url);
            Name = name;
            Description = description;
            UpdateMessage = updateMessage;
        }

        protected override async Task<IDictionary<string, string>> FetchContent(HttpClient client)
        {
            try
            {
                using var rss = await client.GetAsync(_url);
                var doc = new XmlDocument();
                doc.LoadXml(await rss.Content.ReadAsStringAsync());

                var items = doc["rss"]["channel"].GetElementsByTagName("item");

                return items.Cast<XmlNode>()
                    .ToDictionary(node => node["pubDate"]?.InnerText ?? node["title"].InnerText, node => node["link"].InnerText);
            }
            catch(Exception e)
            {
                throw new FetchFailedException($"Failed to fetch content from {_url}", e);
            }
        }
    }
}