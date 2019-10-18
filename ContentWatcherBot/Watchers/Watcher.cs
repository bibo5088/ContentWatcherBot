using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
{
    public abstract class Watcher
    {
        private static readonly HttpClient HttpClient = new HttpClient();

        /// <summary>
        /// Name used to select this watcher, must be unique for every watcher, alpha num only
        /// </summary>
        /// <example>super_duper_webcomic</example>
        public abstract string Name { get; }

        /// <summary>
        /// Description for this watcher
        /// </summary>
        /// <example>Super Duper Webcomic</example>
        public abstract string Description { get; }

        /// <summary>
        /// Message that will be sent when new content shows up
        /// </summary>
        /// <example>New page of Super Duper Webcomic !</example>
        protected abstract string UpdateMessage { get; }

        /// <summary>
        /// Previous fetched IDs, used to detect new content
        /// </summary>
        private HashSet<string> _previousContentIds;

        private Watcher()
        {
        }

        /// <summary>
        /// Build a new watcher and fill its _previousContentIds
        /// </summary>
        /// <typeparam name="T">A class derived from Watcher</typeparam>
        /// <returns>Watcher of class T</returns>
        /// <example><![CDATA[var superDuperWebcomicWatcher = Watcher.Build<SuperDuperWebcomicWatcher>()]]></example>
        public static async Task<T> Build<T>() where T : Watcher, new()
        {
            var watcher = new T();
            var content = await watcher.FetchContent(HttpClient);

            watcher._previousContentIds = content.Keys.ToHashSet();

            return watcher;
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
        public async Task<IEnumerable<string>> CheckAndGetMessages()
        {
            return (await NewContent()).Select(content => $"{UpdateMessage}\n${content}");
        }
    }
}