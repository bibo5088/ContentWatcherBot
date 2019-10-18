using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
{
    public abstract class Watcher
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        public abstract string Name { get; }
        public abstract string Description { get; }
        protected abstract string UpdateMessage { get; }

        private HashSet<string> _previousContentIds;

        private Watcher()
        {
        }

        public static async Task<T> Build<T>() where T : Watcher, new()
        {
            var watcher = new T();
            var content = await watcher.FetchContent(_httpClient);

            watcher._previousContentIds = content.Keys.ToHashSet();

            return watcher;
        }

        protected abstract Task<IDictionary<string, string>> FetchContent(HttpClient client);

        private async Task<IEnumerable<string>> NewContent()
        {
            var content = await FetchContent(_httpClient);

            //Filter out previous content
            var newContentKeysEnumerable = content.Keys.Except(_previousContentIds);
            var newContentKeys = newContentKeysEnumerable as string[] ?? newContentKeysEnumerable.ToArray();
            //Update _previousContent if needed
            if (newContentKeys.Any())
            {
                _previousContentIds = newContentKeys.ToHashSet();
            }

            return newContentKeys.Select(key => content[key]);
        }

        public async Task<IEnumerable<string>> CheckAndGetMessages()
        {
            return (await NewContent()).Select(content => $"{UpdateMessage}\n${content}");
        }
    }
}