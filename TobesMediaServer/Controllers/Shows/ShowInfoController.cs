using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;
using TobesMediaServer.MediaInfo;

namespace TobesMediaServer.Controllers.Shows
{
    [Route("api/")]
    [ApiController]
    public class ShowInfoController : ControllerBase
    {
        private IShowInfo m_showInfo;

        public ShowInfoController(IShowInfo movieInfo)
        {
            m_showInfo = movieInfo;
        }

        [Route("media/get/shows/{name}/{page}/{checkDownload}")]
        public async Task<string> GetShowsByNameAsync(string name, int page, bool checkDownload)
        {
            MediaPage movies = await m_showInfo.GetMediaByNameAsync(name, page, checkDownload);
            return JsonConvert.SerializeObject(movies);
        }
    }
}
