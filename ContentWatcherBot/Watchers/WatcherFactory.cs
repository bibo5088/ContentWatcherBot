using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
{
    public static class WatcherFactory
    {
        public static async Task<Watcher> GetWatcher(string type, string[] args)
        {
            Watcher watcher;

            switch (type)
            {
                case "rss_feed":
                    watcher = new RSSFeedWatcher();
                    break;

                default:
                    throw new ReportableException($"Watcher {type} does not exist");
            }

            await watcher.FillPreviousContentId();

            return watcher;
        }
    }
}