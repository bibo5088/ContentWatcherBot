using System;
using System.Collections.Generic;

namespace ContentWatcherBot.Database
{
    public class Server
    {
        public int Id { get; set; }

        public ulong DiscordId { get; set; }

        public List<ServerWatcher> ServerWatchers { get; set; }
    }
}