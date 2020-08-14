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

        public async Task<string> GetMovieLinkByNzbIdAsync(string id)
        {
            string url = "https://api.nzbgeek.info/api?t=movie&q=1080p&maxsize=5242880000&imdbid=" + id.Replace("tt", "").Replace("TT", "") + "&limit=50&o=json&apikey=3d98d8eaf835802e503a0a936f37ce7c";
            return await GetResultAsync(url, id);
        }

        public async Task<string> GetShowLinkByNzbIdAsync(string id)
        {
            string url = "https://api.nzbgeek.info/api?t=tvsearch&q=1080p&maxsize=5242880000&tvdbid=" + id.Replace("tt", "").Replace("TT", "") + "&limit=50&o=json&apikey=3d98d8eaf835802e503a0a936f37ce7c";
            return await GetResultAsync(url, id);
        }

        private async Task<string> GetResultAsync(string url, string id)
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
                string link = array[0]["link"].ToString().Replace(";", "&");
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
