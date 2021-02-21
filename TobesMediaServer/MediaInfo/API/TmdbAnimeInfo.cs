using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Indexer;
using TobesMediaServer.MediaInfo.OMDB;

namespace TobesMediaServer.MediaInfo.API
{
    public class TmdbAnimeInfo : IAnimeInfo
    {
        HttpClient m_client = new HttpClient();
        private const string m_basePosterURL = "https://image.tmdb.org/t/p/original";

        private IUsenetIndexer m_indexer;
        private ThetvdbAnimeInfo m_tvdbInfo;

        public TmdbAnimeInfo(IUsenetIndexer indexer)
        {
            m_indexer = indexer;
            m_tvdbInfo = new ThetvdbAnimeInfo(indexer);
        }

        public async Task<MediaBase> GetMediaByIDAsync(string id)
        {
            var message = await m_client.GetAsync("https://api.themoviedb.org/3/tv/" + id + "?api_key=295f15628e2e84110ce9197ae94e652b");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            MediaBase movie = await ParseMediaFromJsonAsync(json, false);
            return movie;
        }

        public async Task<MediaPage> GetMediaByNameAsync(string name, int page, bool checkDownload)
        {
            Console.WriteLine($"Searching Page {page} for {name}");
            List<MediaBase> anime = new List<MediaBase>();
            name = name.Replace(" ", "%20");
            var message = await m_client.GetAsync($"https://api.themoviedb.org/3/search/tv?api_key=295f15628e2e84110ce9197ae94e652b&language=en-US&query={name}&page={page}");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (!json.ContainsKey("results"))
                return null;
            JArray array = JArray.Parse(json["results"].ToString());
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < array.Count; i++)
            {
                tasks.Add(ParseMediaFromJsonAsync(array[i] as JObject, checkDownload, anime));
            }
            await Task.WhenAll(tasks);
            return new MediaPage(1, page, anime);
        }

        private async Task<MediaBase> ParseMediaFromJsonAsync(JObject json, bool checkDownload, List<MediaBase> mediaList = null)
        {
            string id = json["id"].ToString();
            var message = await m_client.GetAsync($"https://api.themoviedb.org/3/tv/{id}/external_ids?api_key=295f15628e2e84110ce9197ae94e652b");

            JObject result = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (!result.ContainsKey("tvdb_id"))
                return null;
            string tvdbid = result["tvdb_id"].ToString();
            if (tvdbid == null || tvdbid == string.Empty)
                return null;
            bool? isAnime = await m_tvdbInfo.IsAnimeAsync(tvdbid);
            if (!isAnime.HasValue)
                return null;
            if (isAnime.Value != true)
                return null;
            if (checkDownload && !await m_indexer.DoesShowExistAsync(tvdbid))
                return null;
            string posterPath = json["poster_path"].ToString();
            if (posterPath == null || posterPath == string.Empty)
                return null;
            MediaBase media = new MediaBase(json["name"].ToString(), "", m_basePosterURL + json["poster_path"].ToString(), id, tvdbid);
            if (mediaList != null)
                mediaList.Add(media);
            return media;
        }
    }
}
