using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Diagnostics;
using System;
using Newtonsoft.Json;
using TobesMediaCommon.Data.Media;

namespace TobesMediaCore.Data.Media
{
    [System.Serializable]
    public class MediaBase
    {
        [JsonProperty]
        public string Name { get; private set; } = string.Empty;
        [JsonProperty]
        public string Description { get; private set; } = string.Empty;
        [JsonProperty]
        public string PosterURL { get; private set; } = string.Empty;
        [JsonProperty]
        public string ID { get; private set; } = string.Empty;

        public MediaBase() { }

        public MediaBase(string name, string description, string posterURL, string id)
        {
            Name = name;
            Description = description;
            PosterURL = posterURL;
            ID = id;
        }

        //public async Task LoadMovieAsync(string imdbID, HttpClient client)
        //{
        //    HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movie/" + imdbID);
        //    string jsonObj = await response.Content.ReadAsStringAsync();
        //    MediaBase mediaBase = JsonConvert.DeserializeObject<MediaBase>(jsonObj);
        //    Name = mediaBase.Name;
        //    Description = mediaBase.Description;
        //    PosterURL = mediaBase.PosterURL;
        //    ID = mediaBase.ID;
        //    IsDownloaded = mediaBase.IsDownloaded;
        //}

        public async Task<MediaStatus> GetStatus(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movie/status/" + ID);
            MediaStatus status = (MediaStatus)await response.Content.ReadAsAsync(typeof(MediaStatus));
            return status;
        }

        public async Task DownloadMovie(HttpClient client)
        {
            string id = ID.Replace("tt", "");
            await client.PutAsync("https://localhost:5001/api/media/request/movie/" + id, null);
        }
    }
}
