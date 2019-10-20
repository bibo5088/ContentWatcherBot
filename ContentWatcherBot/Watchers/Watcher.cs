using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
{
    public abstract class Watcher
    {
        private static HttpClient HttpClient = new HttpClient();

        /// <example>super_duper_webcomic</example>
        public string Name { get; protected set; }

        /// <example>Watch for new pages of Super Duper Webcomic</example>
        public string Description { get; protected set; }

        /// <summary>
        /// Message that will be sent when new content shows up
        /// </summary>
        /// <example>New page of Super Duper Webcomic !</example>
        public string UpdateMessage { get; protected set; }

        /// <summary>
        /// Previous fetched IDs, used to detect new content
        /// </summary>
        private HashSet<string> _previousContentIds;

        /// <summary>
        /// Fetch content from source and fill _previousContentIds, should always be called after initialization
        /// </summary>
        public async Task FirstFetch()
        {
            var content = await FetchContent(HttpClient);

            _previousContentIds = content.Keys.ToHashSet();
        }

        /// <summary>
        /// Fetch content from the source
        /// </summary>
        /// <param name="client"></param>
        /// <returns>A dictionary, keys are ids and values are the content</returns>
        protected abstract Task<IDictionary<string, string>> FetchContent(HttpClient client);

        /// <summary>
        /// Fetch content from the source and filters out already known content, updates _previousContentIds if new content is found 
        /// </summary>
        /// <returns>A list of new content</returns>
        private async Task<IEnumerable<string>> NewContent()
        {
            var content = await FetchContent(HttpClient);

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

        /// <summary>
        /// Fetch new content from the source and prepend UpdateMessage to them
        /// </summary>
        /// <returns>List of messages to send</returns>
        public async Task<string[]> CheckAndGetMessages()
        {
            return (await NewContent()).Select(content => $"{UpdateMessage}\n{content}").ToArray();
        }
    }
}