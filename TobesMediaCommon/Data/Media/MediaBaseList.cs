using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;

namespace TobesMediaCore.Data.Media
{
    public enum MediaType { Movies, Shows, Anime }

    public class MediaBaseList
    {
        public int Count { get; private set; }
        public List<MediaBase> List = new List<MediaBase>();
        public int Pages { get; private set; }

        public async Task LoadMediaByName(string name, int page, HttpClient client, bool checkDownloads = false)
        {
            string type = "anime";
            HttpResponseMessage response = await client.GetAsync($"https://localhost:5001/api/media/get/{type}/{name}/{page}/{checkDownloads}");
            if (!response.IsSuccessStatusCode)
                return;
            string jsonObj = await response.Content.ReadAsStringAsync();
            MediaPage mediaPage = JsonConvert.DeserializeObject<MediaPage>(jsonObj);
            Pages = mediaPage.Pages;
            Console.WriteLine("Movies: " + mediaPage.Media.Count);
            Count = mediaPage.Media.Count;
            foreach (MediaBase media in mediaPage.Media)
            {
                List.Add(media);
                await Task.Delay(50);
            }
        }
    }
}
