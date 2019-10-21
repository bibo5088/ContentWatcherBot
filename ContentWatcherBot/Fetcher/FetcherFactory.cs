using System;

namespace ContentWatcherBot.Fetcher
{
    public enum FetcherType
    {
        RssFeed
    }

    public static class FetcherFactory
    {

        public static IFetcher CreateFetcher(FetcherType type, string param)
        {
            return type switch
            {
                FetcherType.RssFeed => CreateRssFeedFetcher(param),
                _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
            };
        }

        private static IFetcher CreateRssFeedFetcher(string param)
        {
            var url = new Uri(param);
            return new RssFeedFetcher(url);
        }
    }
}