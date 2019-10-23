namespace ContentWatcherBot.Database
{
    public class ServerWatcher
    {
        public int Id { get; set; }

        public ulong ChannelId { get; set; }
        
        public int ServerId { get; set; }
        public Server Server { get; set; }

        public int WatcherId { get; set; }
        public Watcher Watcher { get; set; }
    }
}