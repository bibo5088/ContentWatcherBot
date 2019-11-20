using System;
using System.Threading.Tasks;
using ContentWatcherBot.Database;
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

            //Update previousIds
            /*await using (var context = new WatcherContext())
            {
                await context.UpdateWatchers();
            }*/

            Console.WriteLine("Bot is ready");

            await WatchLoop(client);
        }

        private static async Task WatchLoop(BaseSocketClient client)
        {
            while (true)
            {
                await Task.Delay(TimeSpan.FromMinutes(1)); //1 Minute

                await using var context = new WatcherContext();
                var channelMessages = await context.GetNewContentMessages();

                foreach (var channelId in channelMessages.Keys)
                {
                    var channel = (ISocketMessageChannel) client.GetChannel(channelId);

                    //Ignore if channel is null
                    if (channel == null) continue;

                    //Send messages
                    foreach (var message in channelMessages[channelId])
                    {
                        await channel.SendMessageAsync(message);
                    }
                }
            }
        }
    }
}
