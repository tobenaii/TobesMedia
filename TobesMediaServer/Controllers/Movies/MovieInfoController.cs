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
using TobesMediaServer.MediaPipeline;
using TobesMediaServer.OMDB;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MovieInfoController : ControllerBase
    {
        private IMovieInfo m_movieInfo;
        private ILocalMediaDatabase m_database;
        private IPipelineData m_pipelineData;

        public MovieInfoController(IMovieInfo movieInfo, ILocalMediaDatabase database, IPipelineData data)
        {
            m_movieInfo = movieInfo;
            m_database = database;
            m_pipelineData = data;
        }

        [Route("media/get/movies/{name}/{page}/{checkDownload}")]
        public async Task<string> GetMoviesByNameAsync(string name, int page, bool checkDownload)
        {
            MediaPage movies = await m_movieInfo.GetMediaByNameAsync(name, page, checkDownload);
            return JsonConvert.SerializeObject(movies);
        }

        [Route("media/get/movies/isAvailable/{id}")]
        public async Task<bool> GetIsMovieAvailableAsync(string id)
        {
            return await m_database.MediaExistsAsync(id);
        }

        [Route("media/get/movie/status/{id}")]
        public string GetMediaStatus(string id)
        {
            MediaStatus? status = m_pipelineData.GetStatus(id);
            if (status.HasValue)
                return JsonConvert.SerializeObject(status.Value);
            return JsonConvert.SerializeObject(new MediaStatus());
        }
    }
}
