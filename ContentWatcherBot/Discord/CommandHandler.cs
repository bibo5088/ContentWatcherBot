using System;
using System.Reflection;
using System.Threading.Tasks;
using ContentWatcherBot.Discord.Commands;
using Discord;
using Discord.Commands;
using Discord.WebSocket;

namespace ContentWatcherBot.Discord
{
    public class CommandHandler
    {
        private readonly DiscordSocketClient _client;
        private readonly CommandService _commands;

        public CommandHandler(DiscordSocketClient client, CommandService commands)
        {
            _commands = commands;
            _client = client;
        }

        public async Task Setup()
        {
            _client.MessageReceived += HandleCommandAsync;

            _commands.AddTypeReader(typeof(Uri), new UriTypeReader());

            //Load commands
            await _commands.AddModuleAsync<HelpModule>(null);
            await _commands.AddModuleAsync<ListModule>(null);
            await _commands.AddModuleAsync<AddModule>(null);
            await _commands.AddModuleAsync<RemoveModule>(null);
            await _commands.AddModuleAsync<SetModule>(null);
        }

        private async Task HandleCommandAsync(SocketMessage messageParam)
        {
            // Don't process the command if it was a system message
            if (!(messageParam is SocketUserMessage message)) return;

            var argPos = 0;

            // Determine if the message is a command based on the prefix and make sure no bots trigger commands
            if (!(message.HasStringPrefix(Environment.GetEnvironmentVariable("PREFIX"), ref argPos) ||
                  message.HasMentionPrefix(_client.CurrentUser, ref argPos)) ||
                message.Author.IsBot)
                return;

            var context = new SocketCommandContext(_client, message);

            var result = await _commands.ExecuteAsync(context, argPos, null);

            //Error reporting
            if (!result.IsSuccess && result.Error == CommandError.Exception && result is ExecuteResult execResult &&
                execResult.Exception is ReportableExceptions e)
            {
                await context.Channel.SendMessageAsync($":x:{e.Message}");
            }
            else if (!string.IsNullOrEmpty(result.ErrorReason))
            {
                await context.Channel.SendMessageAsync($":x:{result.ErrorReason}");
            }
        }
    }
}