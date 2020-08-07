using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using TobesMediaCore.Data.Media;
using TobesMediaServer.MediaInfo;
using TobesMediaServer.MediaPipeline;
using TobesMediaServer.MediaRequest;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MovieRequestController : ControllerBase
    {
        public IMovieInfo m_movieInfo;
        public IMediaPipeline m_mediaPipeline;

        public MovieRequestController(IMediaPipeline mediaPipeline, IMovieInfo movieInfo)
        {
            m_movieInfo = movieInfo;
            m_mediaPipeline = mediaPipeline;
        }

        [Route("media/request/movie/{id}")]
        public async Task RequestMovieByIDAsync(string id)
        {
            MediaBase media = await m_movieInfo.GetMediaByIDAsync(id);
            await m_mediaPipeline.ProcessMediaAsync(media);
        }
    }
}
