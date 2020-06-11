using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaCore.Data.Media
{
    public class MediaBaseList
    {
        public List<MediaBase> List = new List<MediaBase>();

        public async Task LoadMoviesByName(string name, HttpClient client)
        {
            HttpResponseMessage response = await client.GetAsync("https://localhost:5001/api/media/get/movies/" + name);
            string jsonObj = await response.Content.ReadAsStringAsync();
            List<string> jsonObjList = JsonConvert.DeserializeObject<List<string>>(jsonObj);
            foreach (string json in jsonObjList)
            {
                MediaBase mediaBase = JsonConvert.DeserializeObject<MediaBase>(json);
                List.Add(mediaBase);
            }
        }
    }
}
