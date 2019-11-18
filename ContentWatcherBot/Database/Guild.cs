using System.Collections.Generic;

namespace ContentWatcherBot.Database
{
    public class Guild
    {
        public int Id { get; set; }

        public ulong GuildId { get; set; }

        public List<Hook> Hooks { get; set; }
    }
}