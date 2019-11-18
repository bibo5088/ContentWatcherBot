using System;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace ContentWatcherBot.Database.Watchers
{
    public class RssFeedWatcher : UrlWatcher
    {
        private RssFeedWatcher()
        {
            Type = WatcherType.RssFeed;
        }

        public RssFeedWatcher(Uri url) : base(url)
        {
            Type = WatcherType.RssFeed;
        }

        public override async Task<FetchResult> Fetch()
        {
            using var rss = await HttpClient.GetAsync(Url);
            rss.EnsureSuccessStatusCode();
            var doc = new XmlDocument();
            doc.LoadXml(await rss.Content.ReadAsStringAsync());

            var channel = doc["rss"]["channel"];

            //Title
            var title = channel["title"]?.InnerText ?? Url.ToString();

            //Description
            var description = channel["description"]?.InnerText ?? $"RSS feed {Url}";

            //Content
            var items = channel.GetElementsByTagName("item");

            var content = items.Cast<XmlNode>()
                .ToDictionary(node => node["guid"]?.InnerText ?? node["pubDate"]?.InnerText ?? node["title"].InnerText,
                    node => node["link"].InnerText);

            return new FetchResult(title, description, content);
        }
    }
}