using System;
using System.Threading.Tasks;
using Discord.Commands;
using Discord.WebSocket;

namespace ContentWatcherBot.Discord.Commands
{
    public class AddModule : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Add a watcher to your server")]
        public async Task SquareAsync([Summary("Url to watch")] Uri url,
            [Summary("The channel where the watcher will send messages")]
            SocketGuildChannel channel)
        {
            if (!(Context.User is SocketGuildUser user))
            {
                throw new ServerOnlyCommand();
            }

            if (!user.GuildPermissions.Administrator)
            {
                throw new AdminOnlyCommand();
            }
        }
    }
}