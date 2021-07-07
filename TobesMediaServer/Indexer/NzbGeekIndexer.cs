using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TobesMediaServer.Indexer
{
    public class NzbGeekIndexer : IUsenetIndexer
    {
        private HttpClient m_client = new HttpClient();
        private Dictionary<string, int> _scores = new Dictionary<string, int>()
        {
            {"1080p", 10 },
            {"bluray", 10 },
            {"h.265", 20 },
            {"h265", 20 },
            {"720p", 5 }
        };
        public List<string> BannedLinks { get; } = new List<string>();

        public async Task<bool> DoesShowExistAsync(string id)
        {
            string url = "https://api.nzbgeek.info/api?t=tvsearch&tvdbid=" + id.Replace("tt", "").Replace("TT", "") + "&limit=50&o=json&apikey=QT84i2ZgiimSUutChk9e7f7t3FcRzRyv";
            string results = await GetResultAsync(url, id);
            return results != string.Empty;
        }

        public async Task<string> GetShowLinkByNzbIdAsync(string id, int season, int episode, int downloadIndex)
        {
            string url = "https://api.nzbgeek.info/api?t=tvsearch&q=" + "S" + (season < 10 ? "0":"") + season + "E" + (episode < 10 ? "0" : "") + episode + "&tvdbid=" + id.Replace("tt", "").Replace("TT", "") + "&limit=50&o=json&apikey=QT84i2ZgiimSUutChk9e7f7t3FcRzRyv";
            return await GetResultAsync(url, id);
        }

        private struct Scorer
        {
            public int score;
            public string link;
        }

        private async Task<string> GetResultAsync(string url, string id, int index = 0)
        {
            var message = await m_client.GetAsync(url);
            string json = await message.Content.ReadAsStringAsync();
            try
            {
                JObject jsonNZB = JObject.Parse(json);
                JToken token = jsonNZB["channel"]["item"];
                if (token == null)
                    return string.Empty;
                JArray array = JArray.Parse(token.ToString());
                if (index >= array.Count)
                    return string.Empty;
                List<Scorer> links = new List<Scorer>();
                for (int i = 0; i < array.Count; i++)
                {
                    string link = array[i]["link"].ToString().Replace(";", "&");
                    string title = array[i]["title"].ToString().ToLower();
                    if (BannedLinks.Contains(link))
                        continue;
                    var score = new Scorer();
                    score.link = link;
                    foreach (var scoreCheck in _scores.Keys)
                    {
                        if (title.Contains(scoreCheck))
                            score.score += _scores[scoreCheck];
                    }
                    links.Add(score);
                }
                if (links.Count == 0)
                    return string.Empty;
                return links.OrderByDescending(x => x.score).ElementAt(0).link;
            }
            catch
            {
                Console.WriteLine("Json is invalid: ");
                Console.WriteLine(json);
                return string.Empty;
            }
        }
    }
}
