using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ContentWatcherBot.Fetcher;

namespace ContentWatcherBot.Database.Watchers
{
    public abstract class Watcher
    {
        public static HttpClient HttpClient = new HttpClient();

        public int Id { get; set; }
        public abstract FetcherType Type { get; }
        public string Title { get; private set; }
        public string Description { get; private set; }

        /// <summary>
        /// Previous fetched IDs, used to detect new content
        /// </summary>
        public HashSet<string> PreviousContentIds { get; private set; }

        public List<Hook> Hooks { get; private set; }

        protected abstract Task<FetchResult> Fetch();

        private async Task<Dictionary<string, string>> FetchContent()
        {
            var result = await Fetch();
            Title = result.Title;
            Description = result.Description;
            return result.Content;
        }

        /// <summary>
        /// Fetch content from the source and filters out already known content, updates _previousContentIds if new content is found 
        /// </summary>
        /// <returns>A list of new content</returns>
        public async Task<IEnumerable<string>> NewContent()
        {
            var content = await FetchContent();

            //Filter out previous content
            var newContentKeysEnumerable = content.Keys.Except(PreviousContentIds);
            var newContentKeys = newContentKeysEnumerable as string[] ?? newContentKeysEnumerable.ToArray();
            //Update PreviousContentIds if needed
            if (newContentKeys.Any())
            {
                PreviousContentIds = content.Keys.ToHashSet();
            }

            return newContentKeys.Select(key => content[key]);
        }
    }

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
}