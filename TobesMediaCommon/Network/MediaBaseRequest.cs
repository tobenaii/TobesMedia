using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaCore.Network
{
    public class MediaBaseRequest : ServerRequest
    {
        public async Task<MediaBase> GetMedia(string imdbID, HttpClient client)
        {
            var message = await client.GetAsync("https://www.omdbapi.com/?i=" + imdbID + "&apikey=9c95fc1d");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            MediaBase movie = ParseMediaFromJson(json);
            return movie;
        }

        public async Task<string> GetNZB(string imdbID, HttpClient client)
        {
            var message = await client.GetAsync("https://api.nzbgeek.info/api?t=movie&q=1080p&maxsize=5242880000&imdbid=" + imdbID.Replace("TT", "") + "&limit=50&o=json&apikey=3d98d8eaf835802e503a0a936f37ce7c");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            JArray array = JArray.Parse(json["channel"]["item"].ToString());
            return array[0]["link"].ToString().Replace(";", "&");
        }

        public MediaBase ParseMediaFromJson(JObject json)
        {
            return new MediaBase(json["Title"].ToString(), "", json["Poster"].ToString(), json["imdbID"].ToString());
        }

        public async Task<List<MediaBase>> GetMoviesByName(string name, HttpClient client)
        {
            List<MediaBase> movies = new List<MediaBase>();
            name = name.Replace(" ", "+");
            var message = await client.GetAsync("https://www.omdbapi.com/?s=" + name + "&apikey=9c95fc1d");
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
    }
}
