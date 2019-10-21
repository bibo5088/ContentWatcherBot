using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Xml;

namespace ContentWatcherBot.Fetcher
{
    public class RssFeedFetcher : IFetcher
    {
        private readonly Uri _url;

        public RssFeedFetcher(Uri url)
        {
            _url = url;
        }

        public async Task<FetchResult> FetchContent(HttpClient client)
        {
            using var rss = await client.GetAsync(_url);
            var doc = new XmlDocument();
            doc.LoadXml(await rss.Content.ReadAsStringAsync());

            var channel = doc["rss"]["channel"];

            //Title
            var title = channel["title"]?.InnerText ?? _url.ToString();

            //Description
            var description = channel["description"]?.InnerText ?? $"RSS feed {_url}";

            //Content
            var items = channel.GetElementsByTagName("item");

            var content = items.Cast<XmlNode>()
                .ToDictionary(node => node["pubDate"]?.InnerText ?? node["title"].InnerText,
                    node => node["link"].InnerText);

            return new FetchResult(title, description, content);
        }
    }
}