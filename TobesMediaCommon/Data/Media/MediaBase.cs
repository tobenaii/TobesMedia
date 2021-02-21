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
        public class Season
        {
            public int Episodes { get; private set; }

            public Season(int episodes)
            {
                Episodes = episodes;
            }
        }

        [JsonProperty]
        public string Name { get; private set; } = string.Empty;
        [JsonProperty]
        public string Description { get; private set; } = string.Empty;
        [JsonProperty]
        public string PosterURL { get; private set; } = string.Empty;
        [JsonProperty]
        public string SearchID { get; private set; } = string.Empty;
        [JsonProperty]
        public string ID { get; private set; } = string.Empty;
        public bool IsAvailable { get; private set; } = false;
        public MediaStatus Status { get; private set; }
        public string MediaURL { get; private set; }

        public MediaBase() { }

        public MediaBase(string name, string description, string posterURL, string id, string searchId)
        {
            Name = name;
            Description = description;
            PosterURL = posterURL;
            searchId = searchId.Replace("tt", "");
            SearchID = searchId;
            ID = id;
        }

        public async Task UpdateStatus(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/anime/status/" + SearchID);
            string statusJson = await response.Content.ReadAsStringAsync();
            MediaStatus status = JsonConvert.DeserializeObject<MediaStatus>(statusJson);
            Status = status;
        }

        public void Download(HttpClient client, int season, int episode)
        {
            client.PutAsync("https://localhost:5001/api/media/request/anime/" + ID + "/" + season + "/" + episode, null);
        }

        public async Task UpdateAvailability(HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movies/isAvailable/" + SearchID);
            IsAvailable = await response.Content.ReadAsAsync<bool>();
        }
    }
}
