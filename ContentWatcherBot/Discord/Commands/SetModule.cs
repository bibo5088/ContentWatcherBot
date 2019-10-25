using System.Threading.Tasks;
using ContentWatcherBot.Database;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot.Discord.Commands
{
    [Group("set")]
    public class SetModule : ModuleBase<SocketCommandContext>
    {
        [Command("message")]
        [Summary("Set the update message of a watcher")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public async Task SetMessageAsync([Summary("The watcher Id")] [RequireOwnsWatcher]
            int id, [Remainder] string message)
        {
            await using var context = new WatcherContext();
            var guildWatcher = await context.GuildWatchers.SingleAsync(gw => gw.Id == id);

            guildWatcher.UpdateMessage = message;

            await context.SaveChangesAsync();

            await Context.Message.Channel.SendMessageAsync(
                $"Updated watcher {id}, new update message set to \"{message}\"");
        }
    }
}