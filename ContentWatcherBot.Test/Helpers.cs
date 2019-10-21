using System.Reflection;
using ContentWatcherBot.Database;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    public static class Helpers
    {
        public static void MockWatcherHttpClient(MockHttpMessageHandler mock)
        {
            var field = typeof(Watcher).GetField("HttpClient", BindingFlags.NonPublic | BindingFlags.Static);
            field.SetValue(null, mock.ToHttpClient());
        }
    }
}