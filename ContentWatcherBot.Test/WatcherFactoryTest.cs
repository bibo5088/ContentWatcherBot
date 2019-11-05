using System;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using ContentWatcherBot.Database;
using ContentWatcherBot.Test.MockResponses;
using NUnit.Framework;
using RichardSzalay.MockHttp;

namespace ContentWatcherBot.Test
{
    [TestFixture]
    public class WatcherFactoryTest
    {
        [TestCase("https://mangadex.com/title/700/", ExpectedResult = "700")]
        [TestCase("https://mangadex.org/title/123/", ExpectedResult = "123")]
        [TestCase("http://mangadex.com/title/87164", ExpectedResult = "87164")]
        [TestCase("http://mangadex.org/title/4657", ExpectedResult = "4657")]
        [TestCase("http://mangadex.org/title/89/haha/dqjrqdq/", ExpectedResult = "89")]
        public string MangadexUrlRegex(string url)
        {
            var regex = (Regex) typeof(WatcherFactory)
                .GetField("MangadexUrlRegex", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);


            return regex.Match(url).Groups[1].Value;
        }
    }
}