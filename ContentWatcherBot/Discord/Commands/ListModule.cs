using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentWatcherBot.Database;
using Discord;
using Discord.Commands;
using Microsoft.EntityFrameworkCore;

namespace ContentWatcherBot.Discord.Commands
{
    public class ListModule : ModuleBase<SocketCommandContext>
    {
        [Command("list")]
        [Summary("List every watcher active on this server")]
        [RequireContext(ContextType.Guild)]
        public async Task ListAsync()
        {
            await using var context = new WatcherContext();

            var guildWatchers = context.GuildWatchers.Where(gw => gw.Guild.GuildId == Context.Guild.Id)
                .Include(gw => gw.Watcher).ToArray();

            //Send default message if there is no watcher
            if (!guildWatchers.Any())
            {
                await Context.Message.Channel.SendMessageAsync("There are no watchers active on this server");
                return;
            }

            foreach (var chunk in guildWatchers.Split(25))
            {
                await SendList(chunk);
            }
        }

        private async Task SendList(IEnumerable<GuildWatcher> guildWatchers)
        {
            var builder = new EmbedBuilder();

            foreach (var guildWatcher in guildWatchers)
            {
                var watcher = guildWatcher.Watcher;

                builder.AddField(new EmbedFieldBuilder
                {
                    Name = $"{guildWatcher.Id} ({watcher.Type:G}) \"{watcher.Title}\"",
                    Value =
                        $"{watcher.Description}\nUpdate Message : {guildWatcher.UpdateMessage}\n<#{guildWatcher.ChannelId}>"
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}