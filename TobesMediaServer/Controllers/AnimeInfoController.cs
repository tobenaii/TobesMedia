using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TobesMediaCommon.Data.Media;
using TobesMediaServer.MediaInfo.API;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class AnimeInfoController : ControllerBase
    {
        private IAnimeInfo m_animeInfo;

        public AnimeInfoController(IAnimeInfo animeInfo)
        {
            m_animeInfo = animeInfo;
        }

        [Route("media/get/anime/{name}/{page}")]
        public async Task<string> GetShowsByNameAsync(string name, int page)
        {
            MediaPage movies = await m_animeInfo.GetMediaByNameAsync(name, page, false);
            return JsonConvert.SerializeObject(movies);
        }
    }
}
