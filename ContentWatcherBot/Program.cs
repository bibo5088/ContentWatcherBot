using System;
using System.Threading.Tasks;
using ContentWatcherBot.Discord;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using dotenv.net;

namespace ContentWatcherBot
{
    internal static class Program
    {
        private static async Task Main(string[] args)
        {
            DotEnv.Config();

            using var client = new DiscordSocketClient();
            await client.LoginAsync(TokenType.Bot, Environment.GetEnvironmentVariable("DISCORD_TOKEN"));
            await client.SetActivityAsync(new Game($"Use {Environment.GetEnvironmentVariable("PREFIX")}"));

            var commandHandler = new CommandHandler(client, new CommandService());
            await commandHandler.Setup();

            await client.StartAsync();

            await Task.Delay(-1);
        }
    }
}