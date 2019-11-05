using System.Linq;
using System.Threading.Tasks;
using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using AngleSharp.Html.Parser;

namespace ContentWatcherBot.Fetcher
{
    public class ItchIoFetcher : IFetcher
    {
        public async Task<FetchResult> FetchContent(string url)
        {
            using var response = await Fetchers.HttpClient.GetAsync(url);
            response.EnsureSuccessStatusCode();
            var parser = new HtmlParser();
            var doc = parser.ParseDocument(await response.Content.ReadAsStreamAsync());

            //Title
            var title = doc.GetElementsByClassName("game_title").First().Text();

            //Description
            var description = doc.QuerySelectorAll(".formatted_description.user_formatted p")
                .Skip(1)
                .First() //Get Second <p>
                .Text();

            //Content
            var devLog = doc.GetElementById("devlog").FindChild<IHtmlUnorderedListElement>();
            var content = devLog.Children.ToDictionary(
                el => el.GetElementsByTagName("abbr").First().Attributes["title"].Value,
                el => el.FindChild<IHtmlAnchorElement>().Href);

            return new FetchResult(title, description, content);
        }
    }
}