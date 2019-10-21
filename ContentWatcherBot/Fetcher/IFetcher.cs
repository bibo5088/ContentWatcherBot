using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

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
        Task<FetchResult> FetchContent(HttpClient client);
    }
}