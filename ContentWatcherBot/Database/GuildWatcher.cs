namespace ContentWatcherBot.Database
{
    public class GuildWatcher
    {
        public int Id { get; set; }

        public ulong ChannelId { get; set; }
        
        public int ServerId { get; set; }
        public Guild Guild { get; set; }

        public int WatcherId { get; set; }
        public Watcher Watcher { get; set; }
    }
}