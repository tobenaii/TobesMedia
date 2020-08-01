using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Newtonsoft.Json;
using TobesMediaCore.Data.Media;
using TobesMediaCore.Network;
using TobesMediaServer.Database;
using TobesMediaServer.ffmpeg;
using TobesMediaServer.NZBGet;
using TobesMediaServer.OMDB;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MediaBaseController : ControllerBase
    {
        private static MediaRequest m_mediaBaseRequest = new MediaRequest();
        private static VideoConverter m_videoConverter = new VideoConverter();
        private static MovieDatabase m_movieDatabase = new MovieDatabase();
        private static OmdbManager m_omdbManager = new OmdbManager();

        [Route("media/request/movie/{id}")]
        public async Task RequestMovieByIDAsync(string id)
        {
            MediaBase media = await m_omdbManager.GetMovieByIDAsync(id);
            await m_mediaBaseRequest.DownloadMovieByIDAsync(media);
        }

        [Route("media/play/movie/{id}")]
        public async Task<FileResult> PlayMovieByIDAsync(string id)
        {
            Console.WriteLine(id);
            string movieDir = await m_movieDatabase.GetMovieDirectoryAsync(id);
            PhysicalFileResult file = PhysicalFile(movieDir, "application/x-mpegURL", true);
            return file;
        }

        [Route("media/get/movies/{name}")]
        public async IAsyncEnumerable<string> GetMoviesByNameAsync(string name)
        {
            List<MediaBase> movies = await m_omdbManager.GetMoviesByNameAsync(name);
            foreach (MediaBase media in movies)
            {
                bool movieExists = await m_movieDatabase.MovieExistsAsync(media.ID);
                media.IsDownloaded = movieExists;
                yield return JsonConvert.SerializeObject(media);
            }
        }

        [Route("media/get/movie/progress/{id}")]
        public int GetMovieProgressByID(string id)
        {
            return m_mediaBaseRequest.GetProgress(id);
        }

        [Route("media/get/movie/isDownloading/{id}")]
        public bool GetMovieDownloadingByID(string id)
        {
            return m_mediaBaseRequest.IsDownloading(id);
        }

        [Route("media/get/movie/{id}")]
        public async Task<ActionResult<string>> GetMovieByIDAsync(string id)
        {
            MediaBase media = await m_omdbManager.GetMovieByIDAsync(id);
            bool movieExists = await m_movieDatabase.MovieExistsAsync(media.ID);
            media.IsDownloaded = movieExists;
            return JsonConvert.SerializeObject(media);
        }
    }
}
