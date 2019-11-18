using ContentWatcherBot.Database.Watchers;

namespace ContentWatcherBot.Database
{
    public class Hook
    {
        public int Id { get; set; }

        public ulong ChannelId { get; set; }
        public string UpdateMessage { get; set; }

        public int GuildId { get; set; }
        public Guild Guild { get; set; }

        public int WatcherId { get; set; }
        public Watcher Watcher { get; set; }
    }
}