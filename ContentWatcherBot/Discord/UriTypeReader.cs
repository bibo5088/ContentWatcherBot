using System;
using System.Threading.Tasks;
using Discord.Commands;

namespace ContentWatcherBot.Discord
{
    public class UriTypeReader : TypeReader
    {
        public override Task<TypeReaderResult> ReadAsync(ICommandContext context, string input,
            IServiceProvider services)
        {
            if (Uri.TryCreate(input, UriKind.Absolute, out var result))
                return Task.FromResult(TypeReaderResult.FromSuccess(result));

            return Task.FromResult(TypeReaderResult.FromError(CommandError.ParseFailed,
                "Input could not be parsed as an url"));
        }
    }
}