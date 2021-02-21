using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Indexer;
using TobesMediaServer.MediaInfo.API;

namespace TobesMediaServer.MediaInfo.OMDB
{
    public class ThetvdbAnimeInfo : IAnimeInfo
    {
        HttpClient m_client = new HttpClient();
        private const string m_basePosterURL = "https://thetvdb.com/banners/";

        private IUsenetIndexer m_indexer;

        public ThetvdbAnimeInfo(IUsenetIndexer indexer)
        {
            m_indexer = indexer;
        }

        public Task<MediaBase> GetMediaByIDAsync(string id)
        {
            throw new NotImplementedException();
        }

        public async Task<MediaPage> GetMediaByNameAsync(string name, int page, bool checkDownload)
        {
            await UpdateAuthTokenAsync();
            List<MediaBase> movies = new List<MediaBase>();
            name = name.Replace(" ", "%20");
            var message = await m_client.GetAsync($"https://api.thetvdb.com/search/series?name={name}");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (!json.ContainsKey("data"))
                return null;
            JArray array = JArray.Parse(json["data"].ToString());
            List<Task> tasks = new List<Task>();
            for (int i = 0; i < array.Count; i++)
            {
                tasks.Add(ParseMediaFromJsonAsync(array[i] as JObject, movies));
            }
            await Task.WhenAll(tasks);
            return new MediaPage(array.Count, page, movies);
        }

        public async Task<bool?> IsAnimeAsync(string id)
        {
            await UpdateAuthTokenAsync();
            var message = await m_client.GetAsync($"https://api.thetvdb.com/series/{id}");

            JObject result = JObject.Parse(await message.Content.ReadAsStringAsync());
            if (!result.ContainsKey("data"))
                return null;
            result = JObject.Parse(result["data"].ToString());

            JArray genres = JArray.Parse(result["genre"].ToString());
            foreach (JToken genre in genres.Children())
            {
                if (genre.ToString() == "Anime")
                    return true;
            }
            return false;
        }

        private async Task<MediaBase> ParseMediaFromJsonAsync(JObject json, List<MediaBase> mediaList = null)
        {
            await UpdateAuthTokenAsync();
            string id = json["id"].ToString();
            var message = await m_client.GetAsync($"https://api.thetvdb.com/series/{id}");

            JObject result = JObject.Parse(await message.Content.ReadAsStringAsync());
            result = JObject.Parse(result["data"].ToString());
            if (!result.ContainsKey("id"))
                return null;
            string tvdbid = result["id"].ToString();
            JArray genres = JArray.Parse(result["genre"].ToString());
            foreach (JToken genre in genres.Children())
                if (genre.ToString() == "Anime")
                    return null;
            if (!await m_indexer.DoesShowExistAsync(tvdbid))
            {
                return null;
            }

            if (tvdbid == null || tvdbid == string.Empty)
                return null;
            string posterPath = result["poster"].ToString();
            if (posterPath == null || posterPath == string.Empty)
                return null;
            MediaBase media = new MediaBase(result["seriesName"].ToString(), result["overview"].ToString(), m_basePosterURL + posterPath, id, tvdbid);
            if (mediaList != null)
                mediaList.Add(media);
            return media;
        }

        private async Task UpdateAuthTokenAsync()
        {
            HttpContent content = new StringContent("{\"apikey\": \"dea1727ceb00611c7303c26ed6c12129\"}", Encoding.UTF8, "application/json");
            var message = await m_client.PostAsync($"https://api.thetvdb.com/login", content);
            JObject result = JObject.Parse(await message.Content.ReadAsStringAsync());
            m_client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", result["token"].ToString());
        }
    }
}
