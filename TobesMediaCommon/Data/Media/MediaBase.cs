using System.Net.Http;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using System.Diagnostics;
using System;
using Newtonsoft.Json;

namespace TobesMediaCore.Data.Media
{
    [System.Serializable]
    public class MediaBase
    {
        public string Name;
        public string Description;
        public string PosterURL;
        public string ID;

        public MediaBase() { }

        public MediaBase(string name, string description, string posterURL, string imdbID)
        {
            Name = name;
            Description = description;
            PosterURL = posterURL;
            ID = imdbID;
        }

        public async Task LoadMovieAsync(string imdbID, HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movie/" + imdbID);
            string jsonObj = await response.Content.ReadAsStringAsync();
            MediaBase mediaBase = JsonConvert.DeserializeObject<MediaBase>(jsonObj);
            Name = mediaBase.Name;
            Description = mediaBase.Description;
            PosterURL = mediaBase.PosterURL;
            ID = mediaBase.ID;
        }

        public async Task DownloadMovie(HttpClient client)
        {
            string id = ID.Replace("tt", "");
            await client.PutAsync("https://localhost:5001/api/media/request/movie/" + id, null);
        }
    }
}
