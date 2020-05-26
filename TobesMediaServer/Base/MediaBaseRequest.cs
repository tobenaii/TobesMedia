using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaCore.Network
{
    [Serializable]
    public struct RPCRequest
    {
        public string jsonrpc;
        public int id;
        public string method;
        public object[] parms;
    }

    [Serializable]
    public struct PPParameter
    {
        public string Name;
        public string Value;
    }
    public class MediaBaseRequest : ServerRequest
    {
        private HttpClient m_client = new HttpClient();

        public async Task<MediaBase> GetMovieByIDAsync(string imdbID)
        {
            var message = await m_client.GetAsync("https://www.omdbapi.com/?i=" + imdbID + "&apikey=9c95fc1d");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            MediaBase movie = ParseMediaFromJson(json);
            return movie;
        }

        public async Task DownloadMovieByIDAsync(string imdbID)
        {
            var message = await m_client.GetAsync("https://api.nzbgeek.info/api?t=movie&q=1080p&maxsize=5242880000&imdbid=" + imdbID.Replace("TT", "") + "&limit=50&o=json&apikey=3d98d8eaf835802e503a0a936f37ce7c");
            JObject jsonNZB = JObject.Parse(await message.Content.ReadAsStringAsync());
            JArray array = JArray.Parse(jsonNZB["channel"]["item"].ToString());
            string link = array[0]["link"].ToString().Replace(";", "&");

            RPCRequest request = new RPCRequest();
            request.jsonrpc = "2.0";
            request.id = 1;
            request.method = "append";
            request.parms = new object[]
                {
                    "",
                    link,
                    "",
                    0,
                    true,
                    false,
                    "",
                    0,
                    "SCORE",
                    new object[]{new PPParameter(){Name = "Unpack", Value = "True" } }
                };
            var jsonRequest = JsonConvert.SerializeObject(request);
            jsonRequest = jsonRequest.Replace("parms", "params");
            using (var webClient = new WebClient())
            {
                webClient.UploadString("http://127.0.0.1:6789/jsonrpc/append", jsonRequest);
            }
        }

        public async Task<List<MediaBase>> GetMoviesByNameAsync(string name)
        {
            List<MediaBase> movies = new List<MediaBase>();
            name = name.Replace(" ", "+");
            var message = await m_client.GetAsync("https://www.omdbapi.com/?s=" + name + "&apikey=9c95fc1d");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (!json.ContainsKey("Search"))
                return movies;
            JArray array = JArray.Parse(json["Search"].ToString());
            for (int i = 0; i < array.Count; i++)
            {
                movies.Add(ParseMediaFromJson(array[i] as JObject));
            }
            return movies;
        }

        private MediaBase ParseMediaFromJson(JObject json)
        {
            return new MediaBase(json["Title"].ToString(), "", json["Poster"].ToString(), json["imdbID"].ToString());
        }
    }
}
