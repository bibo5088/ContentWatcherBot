using System;
using System.Threading.Tasks;
using ContentWatcherBot.Database;
using ContentWatcherBot.Database.Watchers;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ContentWatcherBot.Discord.Commands
{
    public class AddModule : ModuleBase<SocketCommandContext>
    {
        [Command("add")]
        [Summary("Add a watcher to the server")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public async Task AddAsync([Summary("Url to watch")] Uri url,
            [Summary("The channel where the watcher will send messages")]
            SocketGuildChannel channel)
        {
            await using var context = new WatcherContext();
            var watcher = await context.AddWatcher(WatcherFactory.PickWatcher(url));
            var server = await context.AddGuild(Context.Guild.Id);

            await context.AddHook(server, watcher, channel.Id);

            await Context.Message.Channel.SendMessageAsync($"Watching <{url}> in channel <#{channel.Id}>");
        }
    }
}