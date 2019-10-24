using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using ContentWatcherBot.Database;

namespace ContentWatcherBot.Fetcher
{
    public struct FetchResult
    {
        public readonly string Title;
        public readonly string Description;
        public readonly Dictionary<string, string> Content;

        public FetchResult(string title, string description, Dictionary<string, string> content)
        {
            Title = title;
            Description = description;
            Content = content;
        }
    }

    public interface IFetcher
    {
        Task<FetchResult> FetchContent(string param);
    }

    public enum FetcherType
    {
        RssFeed
    }

    public static class Fetchers
    {
        private static HttpClient _httpClient = new HttpClient();
        public static HttpClient HttpClient => _httpClient;

        public static readonly RssFeedFetcher RssFeedFetcher = new RssFeedFetcher();

        private static readonly Dictionary<FetcherType, IFetcher> FetchersDict = new Dictionary<FetcherType, IFetcher>
        {
            [FetcherType.RssFeed] = RssFeedFetcher
        };

        public static Task<FetchResult> Fetch(FetcherType type, string param)
        {
            return FetchersDict[type].FetchContent(param);
        }
    }
}