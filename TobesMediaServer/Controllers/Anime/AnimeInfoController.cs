using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TobesMediaCommon.Data.Media;
using TobesMediaServer.MediaInfo.API;
using TobesMediaServer.MediaPipeline;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AnimeInfoController : ControllerBase
    {
        private IAnimeInfo m_animeInfo;
        private IPipelineData m_pipelineData;

        public AnimeInfoController(IAnimeInfo animeInfo, IPipelineData pipelineData)
        {
            m_animeInfo = animeInfo;
            m_pipelineData = pipelineData;
        }

        [Route("media/get/anime/{name}/{page}/{checkDownload}")]
        public async Task<string> GetShowsByNameAsync(string name, int page, bool checkDownload)
        {
            MediaPage movies = await m_animeInfo.GetMediaByNameAsync(name, page, checkDownload);
            return JsonConvert.SerializeObject(movies);
        }

        [Route("media/get/anime/status/{id}")]
        public string GetMediaStatus(string id)
        {
            MediaStatus? status = m_pipelineData.GetStatus(id);
            if (status.HasValue)
                return JsonConvert.SerializeObject(status.Value);
            return JsonConvert.SerializeObject(new MediaStatus());
        }
    }
}
