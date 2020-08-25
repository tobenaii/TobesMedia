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

        public bool IsAvailable { get; private set; } = false;
        public MediaStatus Status { get; private set; }

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

        public async Task UpdateStatus(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movie/status/" + ID);
            string statusJson = await response.Content.ReadAsStringAsync();
            MediaStatus status = JsonConvert.DeserializeObject<MediaStatus>(statusJson);
            Status = status;
        }

        public void DownloadMovie(HttpClient client)
        {
            string id = ID.Replace("tt", "");
            client.PutAsync("https://localhost:5001/api/media/request/movie/" + id, null);
        }

        public async Task UpdateAvailability(HttpClient client)
        {
            string id = ID.Replace("tt", "");
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movies/isAvailable/" + id);
            IsAvailable = await response.Content.ReadAsAsync<bool>();
        }
    }
}
