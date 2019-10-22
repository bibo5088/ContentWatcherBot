using System.Reflection;
using ContentWatcherBot.Database;
using ContentWatcherBot.Fetcher;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    public static class Helpers
    {
        public static void MockWatcherHttpClient(MockHttpMessageHandler mock)
        {
            var field = typeof(Fetchers).GetField("_httpClient",  BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, mock.ToHttpClient());
        }
    }
}