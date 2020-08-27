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
        public string MediaURL { get; private set; }

        public MediaBase() { }

        public MediaBase(string name, string description, string posterURL, string id)
        {
            Name = name;
            Description = description;
            PosterURL = posterURL;
            id = id.Replace("tt", "");
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
            client.PutAsync("https://localhost:5001/api/media/request/movie/" + ID, null);
        }

        public async Task UpdateAvailability(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movies/isAvailable/" + ID);
            IsAvailable = await response.Content.ReadAsAsync<bool>();
        }

        public string  GetVideoURL()
        {
            return "https://localhost:5001/api/media/play/movie/" + ID;
        }
    }
}
