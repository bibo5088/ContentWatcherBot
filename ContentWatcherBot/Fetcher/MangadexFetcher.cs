using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace ContentWatcherBot.Fetcher
{
    public class MangadexFetcher : IFetcher
    {
        public async Task<FetchResult> FetchContent(string mangaId)
        {
            using var response = await Fetchers.HttpClient.GetAsync($"https://mangadex.org/api/manga/{mangaId}");
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
                .Where(chapter => chapter.Value.GetProperty("lang_code").GetString() == "gb") //Only english chapter
                .ToDictionary(chapter => chapter.Name, chapter => $"https://mangadex.org/chapter/{chapter.Name}");

            return new FetchResult(title, description, content);
        }
    }
}