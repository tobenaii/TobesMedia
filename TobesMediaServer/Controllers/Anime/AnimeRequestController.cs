using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Database;
using TobesMediaServer.MediaInfo.API;
using TobesMediaServer.MediaPipeline;

namespace TobesMediaServer.Controllers.Anime
{
    [Route("api/")]
    [ApiController]
    public class AnimeRequestController : ControllerBase
    {
        public IAnimeInfo m_animeInfo;
        public IMediaPipeline m_mediaPipeline;
        private ILocalMediaDatabase m_mediaDatabase;

        public AnimeRequestController(IMediaPipeline mediaPipeline, IAnimeInfo animeInfo, ILocalMediaDatabase mediaDatabase)
        {
            m_animeInfo = animeInfo;
            m_mediaPipeline = mediaPipeline;
            m_mediaDatabase = mediaDatabase;
        }

        [Route("media/request/anime/{id}/{season}/{episode}")]
        public async Task RequestAnimeByIDAsync(string id, int season, int episode)
        {
            MediaBase media = await m_animeInfo.GetMediaByIDAsync(id);
            await m_mediaPipeline.ProcessMediaAsync(media, season, episode);
        }

        [Route("media/play/anime/{id}")]
        public async Task<FileResult> PlayMovieByIDAsync(string id)
        {
            Console.WriteLine(id);
            string movieDir = await m_mediaDatabase.GetFilePathAsync(id);
            PhysicalFileResult file = PhysicalFile(movieDir, "video/webm", true);
            return file;
        }
    }
}
