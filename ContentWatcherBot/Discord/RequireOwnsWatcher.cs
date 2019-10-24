using System;
using System.Linq;
using System.Threading.Tasks;
using ContentWatcherBot.Database;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot.Discord
{
    public class RequireOwnsWatcher : ParameterPreconditionAttribute
    {
        public override async Task<PreconditionResult> CheckPermissionsAsync(ICommandContext context,
            ParameterInfo parameter, object value, IServiceProvider services)
        {
            try
            {
                await using var db = new WatcherContext();
                var guild = await db.Guilds.Include(g => g.GuildWatchers)
                    .SingleAsync(g => g.GuildId == context.Guild.Id);
                guild.GuildWatchers.Single(gw => gw.Id == (int) value);

                return PreconditionResult.FromSuccess();
            }
            catch
            {
                return PreconditionResult.FromError($"Watcher `{(int) value}` doesn't not exist on this server");
            }
        }
    }
}