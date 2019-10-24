using System;
using System.Linq;
using System.Threading.Tasks;
using Discord;
using Discord.Commands;

namespace ContentWatcherBot.Discord.Commands
{
    //Taken here https://github.com/Aux/Discord.Net-Example/blob/2.0/src/Modules/HelpModule.cs and modified by me
    public class HelpModule : ModuleBase<SocketCommandContext>
    {
        private readonly CommandService _service;

        public HelpModule(CommandService service)
        {
            _service = service;
        }

        [Command("help")]
        [Summary("List of all the commands")]
        public async Task HelpAsync()
        {
            var prefix = Environment.GetEnvironmentVariable("PREFIX");
            var builder = new EmbedBuilder();

            foreach (var module in _service.Modules)
            {
                foreach (var cmd in module.Commands)
                {
                    if ((await cmd.CheckPreconditionsAsync(Context)).IsSuccess)
                    {
                        var paramsDesc = cmd.Parameters.Aggregate("", (current, param) => current + $" [{param.Name}]");

                        builder.AddField(x =>
                        {
                            x.Name = $"{prefix}{cmd.Aliases.First()}{paramsDesc}";
                            x.Value = cmd.Summary;
                            x.IsInline = false;
                        });
                    }
                }
            }

            await ReplyAsync("", false, builder.Build());
        }

        [Command("help")]
        [Summary("More info on a command")]
        public async Task HelpAsync(string command)
        {
            var result = _service.Search(Context, command);

            if (!result.IsSuccess)
            {
                await ReplyAsync($"Sorry, I couldn't find a command like **{command}**.");
                return;
            }

            string prefix = Environment.GetEnvironmentVariable("PREFIX");
            var builder = new EmbedBuilder()
            {
                Color = new Color(114, 137, 218),
                Description = $"Here are some commands like **{command}**"
            };

            foreach (var match in result.Commands)
            {
                var cmd = match.Command;

                builder.AddField(x =>
                {
                    x.Name = string.Join(", ", cmd.Aliases);
                    x.Value = $"Parameters: {string.Join(", ", cmd.Parameters.Select(p => p.Name))}\n" +
                              $"Summary: {cmd.Summary}";
                    x.IsInline = false;
                });
            }

            await ReplyAsync("", false, builder.Build());
        }
    }
}