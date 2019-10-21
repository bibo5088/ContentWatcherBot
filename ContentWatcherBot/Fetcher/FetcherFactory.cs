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
            if (!(Uri.TryCreate(param, UriKind.Absolute, out var url)
                  && (url.Scheme == Uri.UriSchemeHttp || url.Scheme == Uri.UriSchemeHttps)))
            {
                throw new InvalidWatcherArgumentException($"`{param}` is not a valid url");
            }

            return new RssFeedFetcher(url);
        }
    }
}