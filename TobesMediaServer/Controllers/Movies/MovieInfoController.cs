using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TobesMediaCommon.Data.Media;
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

        public MovieInfoController(IMovieInfo movieInfo)
        {
            m_movieInfo = movieInfo;
        }

        [Route("media/get/movies/{name}/{page}/{checkDownload}")]
        public async Task<string> GetMoviesByNameAsync(string name, int page, bool checkDownload)
        {
            MediaPage movies = await m_movieInfo.GetMediaByNameAsync(name, page, checkDownload);
            return JsonConvert.SerializeObject(movies);
        }
    }
}
