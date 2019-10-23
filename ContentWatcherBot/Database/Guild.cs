using System;
using System.Collections.Generic;

namespace ContentWatcherBot.Database
{
    public class Guild
    {
        public int Id { get; set; }

        public ulong GuildId { get; set; }

        public List<GuildWatcher> GuildWatchers { get; set; }
    }
}