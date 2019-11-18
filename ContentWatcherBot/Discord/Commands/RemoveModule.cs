using System.Threading.Tasks;
using ContentWatcherBot.Database;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot.Discord.Commands
{
    public class RemoveModule : ModuleBase<SocketCommandContext>
    {
        [Command("remove")]
        [Summary("Remove a watcher from the server")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public async Task RemoveAsync([Summary("The watcher Id")] [RequireOwnsWatcher]
            int id)
        {
            await using var context = new WatcherContext();
            var guildWatcher = await context.Hooks.SingleAsync(gw => gw.Id == id);
            context.Hooks.Remove(guildWatcher);

            await context.SaveChangesAsync();

            await Context.Message.Channel.SendMessageAsync($"Watcher `{id}` removed");
        }
    }
}