using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ContentWatcherBot.Watchers;

namespace ContentWatcherBot
{
    public class WatcherInfo
    {
        /// <summary>
        /// Name referring to this watcher for the first time, must be unique for every watcher, alpha num only
        /// </summary>
        /// <example>super_duper_webcomic</example>
        public readonly string Name;

        /// <summary>
        /// Description for this watcher
        /// </summary>
        /// <example>Super Duper Webcomic</example>
        public readonly string Description;

        /// <summary>
        /// Message that will be sent when new content shows up
        /// </summary>
        /// <example>New page of Super Duper Webcomic !</example>
        public readonly string UpdateMessage;

        private readonly Watcher _watcher;

        private WatcherInfo(string name, string description, string updateMessage, Watcher watcher)
        {
            Name = name;
            Description = description;
            UpdateMessage = updateMessage;
            _watcher = watcher;
        }

        public async Task<WatcherInfo> Build(string name, string description, string updateMessage, string type,
            string[] args)
        {
            return new WatcherInfo(name, description, updateMessage, await WatcherFactory.GetWatcher(type, args));
        }

        /// <summary>
        /// Fetch new content from the source and prepend UpdateMessage to them
        /// </summary>
        /// <returns>List of messages to send</returns>
        public async Task<IEnumerable<string>> CheckAndGetMessages()
        {
            return (await _watcher.NewContent()).Select(content => $"{UpdateMessage}\n${content}");
        }
    }
}