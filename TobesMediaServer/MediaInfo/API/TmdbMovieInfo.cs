using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Indexer;
using TobesMediaServer.MediaInfo.OMDB;

namespace TobesMediaServer.MediaInfo.API
{
    public class TmdbMovieInfo : IMovieInfo
    {
        HttpClient m_client = new HttpClient();
        private const string m_basePosterURL = "https://image.tmdb.org/t/p/original";

        private IUsenetIndexer m_indexer;

        public TmdbMovieInfo(IUsenetIndexer indexer)
        {
            m_indexer = indexer;
        }

        public async Task<MediaBase> GetMediaByIDAsync(string id)
        {
            if (id[0] != 't')
                id = "tt" + id;
            var message = await m_client.GetAsync($"https://api.themoviedb.org/3/find/{id}?api_key=295f15628e2e84110ce9197ae94e652b&language=en-US&external_source=imdb_id");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            JArray array = JArray.Parse(json["movie_results"].ToString());
            MediaBase movie = await ParseMediaFromJsonAsync(JObject.Parse(array[0].ToString()));
            return movie;
        }

        public async Task<MediaPage> GetMediaByNameAsync(string name, int page, bool checkDownload)
        {
            List<MediaBase> movies = new List<MediaBase>();
            name = name.Replace(" ", "%20");
            var message = await m_client.GetAsync($"https://api.themoviedb.org/3/search/movie?api_key=295f15628e2e84110ce9197ae94e652b&language=en-US&query={name}&page={page}");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (!json.ContainsKey("results"))
                return null;
            JArray array = JArray.Parse(json["results"].ToString());
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < array.Count; i++)
            {
                tasks.Add(ParseMediaFromJsonAsync(array[i] as JObject, movies));
            }
            await Task.WhenAll(tasks);
            return new MediaPage(json["total_pages"].ToObject<int>(), page, movies);
        }

        private async Task<MediaBase> ParseMediaFromJsonAsync(JObject json, List<MediaBase> mediaList = null)
        {
            string id = json["id"].ToString();
            var message = await m_client.GetAsync($"https://api.themoviedb.org/3/movie/{id}?api_key=295f15628e2e84110ce9197ae94e652b&language=en-US");

            JObject result = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (!result.ContainsKey("imdb_id"))
                return null;
            string imdbID = result["imdb_id"].ToString();

            if (await m_indexer.GetMovieLinkByNzbIdAsync(imdbID) == string.Empty)
            {
                return null;
            }

            if (imdbID == null || imdbID == string.Empty)
                return null;
            string posterPath = json["poster_path"].ToString();
            if (posterPath == null || posterPath == string.Empty)
                return null;
            MediaBase media = new MediaBase(json["title"].ToString(), json["overview"].ToString(), m_basePosterURL + posterPath, imdbID);
            if (mediaList != null)
                mediaList.Add(media);
            return media;
        }
    }
}
