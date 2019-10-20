using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ContentWatcherBot.Watchers
{
    public class WatcherList
    {
        private readonly Dictionary<string, Watcher> _watchers = new Dictionary<string, Watcher>();

        public int Count => _watchers.Count;
        
        public async Task<string> AddWatcher(string name, string? arg = null)
        {
            var watcher = WatcherFactory.CreateWatcher(name, arg);

            //Check if watcher doesn't already exist
            if (!_watchers.ContainsKey(watcher.Name))
            {
                _watchers.Add(watcher.Name, watcher);
                await watcher.FirstFetch();
            }

            return watcher.Name;
        }

        public async Task<ConcurrentDictionary<string, string[]>> CheckAndGetMessages()
        {
            var dict = new ConcurrentDictionary<string, string[]>();

            var check = new Func<Watcher, Task>(async (watch) =>
            {
                try
                {
                    var messages = await watch.CheckAndGetMessages();
                    if (messages.Any())
                    {
                        dict[watch.Name] = messages;
                    }
                }
                catch
                {
                    //Ignore
                }
            });

            var tasks = _watchers.Values.Select(watcher => check(watcher));
            await Task.WhenAll(tasks);
            return dict;
        }

        public void KeepOnly(HashSet<string> toKeep)
        {
            foreach (var key in _watchers.Keys.Where(key => !toKeep.Contains(key)))
            {
                _watchers.Remove(key);
            }
        }
    }
}