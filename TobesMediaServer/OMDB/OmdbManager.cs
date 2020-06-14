using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.OMDB
{
    public class OmdbManager
    {
        HttpClient m_client = new HttpClient();

        public async Task<MediaBase> GetMovieByIDAsync(string imdbID)
        {
            if (imdbID[0] != 't')
                imdbID = "tt" + imdbID;
            var message = await m_client.GetAsync("https://www.omdbapi.com/?i=" + imdbID + "&apikey=9c95fc1d");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            MediaBase movie = ParseMediaFromJson(json);
            return movie;
        }

        private MediaBase ParseMediaFromJson(JObject json)
        {
            return new MediaBase(json["Title"].ToString(), "", json["Poster"].ToString(), json["imdbID"].ToString());
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
    }
}
