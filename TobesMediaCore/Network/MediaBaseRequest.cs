using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaCore.Network
{
    public class MediaBaseRequest : ServerRequest
    {
        public async Task<Movie> GetMovie(string imdbID)
        {
            HttpClient client = new HttpClient();
            var message = await client.GetAsync("http://www.omdbapi.com/?i=" + imdbID + "&apikey=9c95fc1d");
            JObject json = JObject.Parse(await message.Content.ReadAsStringAsync());
            Movie movie = new Movie(json["Title"].ToString(), json["Plot"].ToString(), json["Poster"].ToString());
            return movie;
        }
    }
}
