using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace TobesMediaServer.NZBGeek
{
    public class NzbGeekManager
    {
        private HttpClient m_client;

        public NzbGeekManager()
        {
            m_client = new HttpClient();
        }

        public async Task<string> GetLinkByNzbIdAsync(string id)
        {
            string url = "https://api.nzbgeek.info/api?t=movie&q=1080p&maxsize=5242880000&imdbid=" + id.Replace("tt", "").Replace("TT", "") + "&limit=50&o=json&apikey=3d98d8eaf835802e503a0a936f37ce7c";
            var message = await m_client.GetAsync(url);
            string json = await message.Content.ReadAsStringAsync();
            JObject jsonNZB = JObject.Parse(json);
            JArray array = JArray.Parse(jsonNZB["channel"]["item"].ToString());
            string link = array[0]["link"].ToString().Replace(";", "&");
            return link;
        }
    }
}
