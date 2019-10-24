using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using ContentWatcherBot.Fetcher;

namespace ContentWatcherBot.Database
{
    public class Watcher
    {
        public int Id { get; set; }

        public FetcherType Type { get; private set; }

        public Uri Url { get; private set; }

        public string Param { get; private set; }

        public string Title { get; private set; }
        public string Description { get; private set; }

        public List<GuildWatcher> GuildWatchers { get; private set; }

        /// <summary>
        /// Previous fetched IDs, used to detect new content
        /// </summary>
        public HashSet<string> PreviousContentIds { get; private set; }

        public Watcher(FetcherType type, Uri url, string param, string title, string description,
            HashSet<string> previousContentIds)
        {
            Type = type;
            Url = url;
            Param = param;
            Title = title;
            Description = description;
            PreviousContentIds = previousContentIds;
        }

        private async Task<Dictionary<string, string>> FetchContent()
        {
            var result = await Fetchers.Fetch(Type, Param);
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
            //Update _previousContent if needed
            if (newContentKeys.Any())
            {
                PreviousContentIds = newContentKeys.ToHashSet();
            }

            return newContentKeys.Select(key => content[key]);
        }
    }
}