using System;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContentWatcherBot.Database.Watchers
{
    public class MangadexWatcher : Watcher
    {
        public string MangaId { get; private set; }
        public string Lang { get; set; }

        private MangadexWatcher()
        {
            Type = WatcherType.RssFeed;
        }

        public MangadexWatcher(string mangaId) : this()
        {
            MangaId = mangaId;
            Lang = "gb"; //Default english
        }

        public override async Task<FetchResult> Fetch()
        {
            using var response = await HttpClient.GetAsync($"https://mangadex.org/api/manga/{MangaId}");
            response.EnsureSuccessStatusCode();
            var doc = await JsonDocument.ParseAsync(await response.Content.ReadAsStreamAsync());

            var mangaElement = doc.RootElement.GetProperty("manga");
            var chapterElement = doc.RootElement.GetProperty("chapter");

            //Title
            var title = mangaElement.GetProperty("title").GetString();

            //Description
            var description = mangaElement.GetProperty("description").GetString();

            //Content
            var content = chapterElement.EnumerateObject()
                .Where(chapter => chapter.Value.GetProperty("lang_code").GetString() == Lang)
                .ToDictionary(chapter => chapter.Name, chapter => $"https://mangadex.org/chapter/{chapter.Name}");

            return new FetchResult(title, description, content);
        }

        public override Watcher Clone()
        {
            return new MangadexWatcher {MangaId = MangaId, Lang = Lang};
        }

        public override string Info()
        {
            return $"Lang : {Lang}";
        }

        public override int GetHashCode()
        {
            return Type.GetHashCode() ^ MangaId.GetHashCode() ^ Lang.GetHashCode();
        }
    }
}