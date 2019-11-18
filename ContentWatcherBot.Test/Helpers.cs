using System.Reflection;
using ContentWatcherBot.Database.Watchers;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    public static class Helpers
    {
        public static void MockWatcherHttpClient(MockHttpMessageHandler mock)
        {
            var field = typeof(Watcher).GetField(nameof(Watcher.HttpClient), BindingFlags.Public | BindingFlags.Static);
            field.SetValue(null, mock.ToHttpClient());
        }
    }
}