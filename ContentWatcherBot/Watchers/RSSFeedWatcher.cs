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

        private readonly Uri _url;

        public RssFeedWatcher(string name, string description, string updateMessage, Uri url)
        {
            _url = url;
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