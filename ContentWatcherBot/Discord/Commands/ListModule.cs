using System;
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

            var hook = context.Hooks.AsQueryable().Where(h => h.Guild.GuildId == Context.Guild.Id)
                .Include(h => h.Watcher).ToArray();

            //Send default message if there is no watcher
            if (!hook.Any())
            {
                await Context.Message.Channel.SendMessageAsync("There are no watchers active on this server");
                return;
            }

            foreach (var chunk in hook.Split(25))
            {
                await SendList(chunk);
            }
        }

        private async Task SendList(IEnumerable<Hook> guildWatchers)
        {
            var builder = new EmbedBuilder();

            foreach (var hook in guildWatchers)
            {
                var watcher = hook.Watcher;

                var shortenedWatcherDesc = watcher.Description.Length > 512
                    ? watcher.Description.Substring(0, 512) + '…'
                    : watcher.Description;
                var desc =
                    $"{shortenedWatcherDesc}\nUpdate Message : {hook.UpdateMessage}\n<#{hook.ChannelId}>";

                //Info if any
                var info = watcher.Info();
                if (info.Length > 0)
                {
                    desc += $"\n{info}";
                }

                builder.AddField(new EmbedFieldBuilder
                {
                    Name = $"{hook.Id} ({watcher.Type:G}) \"{watcher.Title}\"",
                    Value = desc
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}