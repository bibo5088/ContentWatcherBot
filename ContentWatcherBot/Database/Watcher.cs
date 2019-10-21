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
        private static HttpClient HttpClient = new HttpClient();

        [NotMapped] private IFetcher _fetcher = null;

        public int Id { get; set; }

        public FetcherType Type { get; private set; }

        public string Param { get; private set; }

        public string Title { get; private set; }
        public string Description { get; private set; }

        public Watcher(FetcherType type, string param)
        {
            Type = type;
            Param = param;
        }

        /// <summary>
        /// Previous fetched IDs, used to detect new content
        /// </summary>
        public HashSet<string> PreviousContentIds { get; private set; }

        private IFetcher GetFetcher()
        {
            return _fetcher ??= FetcherFactory.CreateFetcher(Type, Param);
        }

        private async Task<Dictionary<string, string>> FetchContent(HttpClient httpClient)
        {
            var result = await GetFetcher().FetchContent(httpClient);
            Title = result.Title;
            Description = result.Description;
            return result.Content;
        }

        /// <summary>
        /// Fetch content from source and fill _previousContentIds, should always be called after initialization
        /// </summary>
        public async Task FirstFetch()
        {
            var content = await FetchContent(HttpClient);

            PreviousContentIds = content.Keys.ToHashSet();
        }

        /// <summary>
        /// Fetch content from the source and filters out already known content, updates _previousContentIds if new content is found 
        /// </summary>
        /// <returns>A list of new content</returns>
        public async Task<IEnumerable<string>> NewContent()
        {
            var content = await FetchContent(HttpClient);

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

        #region Equality

        protected bool Equals(Watcher other)
        {
            return Type == other.Type && Param == other.Param;
        }

        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;
            if (obj.GetType() != this.GetType()) return false;
            return Equals((Watcher) obj);
        }

        public override int GetHashCode()
        {
            unchecked
            {
                return ((int) Type * 397) ^ (Param != null ? Param.GetHashCode() : 0);
            }
        }

        #endregion
    }
}