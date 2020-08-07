using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Database;
using TobesMediaServer.MediaInfo;
using TobesMediaServer.OMDB;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MovieInfoController : ControllerBase
    {
        private IMovieInfo m_movieInfo;
        private IMovieDatabase m_movieDatabase;

        public MovieInfoController(IMovieInfo movieInfo, IMovieDatabase movieDatabase)
        {
            m_movieInfo = movieInfo;
            m_movieDatabase = movieDatabase;
        }

        [Route("media/get/movies/{name}")]
        public async IAsyncEnumerable<string> GetMoviesByNameAsync(string name)
        {
            List<MediaBase> movies = await m_movieInfo.GetMediaByNameAsync(name);
            foreach (MediaBase media in movies)
            {
                bool movieExists = await m_movieDatabase.MovieExistsAsync(media.ID);
                media.IsDownloaded = movieExists;
                yield return JsonConvert.SerializeObject(media);
            }
        }

        [Route("media/get/movie/{id}")]
        public async Task<ActionResult<string>> GetMovieByIDAsync(string id)
        {
            MediaBase media = await m_movieInfo.GetMediaByIDAsync(id);
            bool movieExists = await m_movieDatabase.MovieExistsAsync(media.ID);
            media.IsDownloaded = movieExists;
            return JsonConvert.SerializeObject(media);
        }
    }
}
