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

        public async Task<bool> DoesShowExistAsync(string id)
        {
            string url = "https://api.nzbgeek.info/api?t=tvsearch&maxsize=5242880000&tvdbid=" + id.Replace("tt", "").Replace("TT", "") + "&limit=50&o=json&apikey=3d98d8eaf835802e503a0a936f37ce7c";
            string results = await GetResultAsync(url, id);
            return results != string.Empty;
        }

        public async Task<string> GetShowLinkByNzbIdAsync(string id, int season, int episode)
        {
            string url = "https://api.nzbgeek.info/api?t=tvsearch&q=" + "S" + (season < 10 ? "0":"") + season + "E" + (episode < 10 ? "0" : "") + episode + "&maxsize=5242880000&tvdbid=" + id.Replace("tt", "").Replace("TT", "") + "&limit=50&o=json&apikey=3d98d8eaf835802e503a0a936f37ce7c";
            return await GetResultAsync(url, id);
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
                string link = array[index]["link"].ToString().Replace(";", "&");
                return link;
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
