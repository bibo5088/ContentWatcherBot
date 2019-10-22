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
        public async Task<FetchResult> FetchContent(Uri url)
        {
            using var rss = await Fetchers.HttpClient.GetAsync(url);
            var doc = new XmlDocument();
            doc.LoadXml(await rss.Content.ReadAsStringAsync());

            var channel = doc["rss"]["channel"];

            //Title
            var title = channel["title"]?.InnerText ?? url.ToString();

            //Description
            var description = channel["description"]?.InnerText ?? $"RSS feed {url}";

            //Content
            var items = channel.GetElementsByTagName("item");

            var content = items.Cast<XmlNode>()
                .ToDictionary(node => node["pubDate"]?.InnerText ?? node["title"].InnerText,
                    node => node["link"].InnerText);

            return new FetchResult(title, description, content);
        }
    }
}