using System.Threading.Tasks;
using ContentWatcherBot.Database;
using ContentWatcherBot.Database.Watchers;
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
            var hook = await context.Hooks.SingleAsync(h => h.Id == id);

            hook.UpdateMessage = message;

            await context.SaveChangesAsync();

            await Context.Message.Channel.SendMessageAsync(
                $"Updated watcher {id}, new update message set to \"{message}\"");
        }

        [Command("lang")]
        [Summary("Set the lang of a mangadex watcher")]
        [RequireUserPermission(GuildPermission.Administrator)]
        [RequireContext(ContextType.Guild)]
        public async Task SetLangAsync([Summary("The watcher Id")] [RequireOwnsWatcher]
            int id, [Summary(
                "The lang code for the list of every mangadex lang code : https://github.com/frozenpandaman/mangadex-dl/wiki/language-codes")]
            string lang)
        {
            await using var context = new WatcherContext();
            var hook = await context.Hooks.Include(h => h.Watcher).SingleAsync(h => h.Id == id);

            if (hook.Watcher.Type != WatcherType.Mangadex)
            {
                throw new WrongWatcherType(hook.Id, WatcherType.Mangadex, hook.Watcher.Type);
            }

            //Is valid lang code?
            if (!Constants.MangadexLangCodes.Contains(lang))
            {
                throw new ReportableExceptions(
                    $"{lang} is not a valid mangadex lang code\nList of every mangadex lang code : https://github.com/frozenpandaman/mangadex-dl/wiki/language-codes");
            }

            //Create new watcher with lang
            var oldWatcher = (MangadexWatcher) hook.Watcher;
            var newWatcher = (MangadexWatcher) oldWatcher.Clone();
            newWatcher.Lang = lang;

            //Save to DB
            await context.AddWatcher(newWatcher);
            hook.Watcher = newWatcher;
            hook.WatcherId = newWatcher.Id;
            await context.SaveChangesAsync();

            await Context.Message.Channel.SendMessageAsync(
                $"Updated watcher {id}, lang set to \"{lang}\"");
        }
    }
}