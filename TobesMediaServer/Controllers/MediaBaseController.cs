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
using TobesMediaServer.NZBGet;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MediaBaseController : ControllerBase
    {
        private MediaRequest m_mediaBaseRequest = new MediaRequest();

        [Route("media/request/movie/{id}")]
        public async Task RequestMovieByIDAsync(string id)
        {
            await m_mediaBaseRequest.DownloadMovieByIDAsync(id);
        }

        [Route("media/play/movie/{id}")]
        public async Task<FileResult> PlayMovieByIDAsync(string id)
        {
            Console.WriteLine(id);
            string movieDir;
            if (id[0] == 't')
            {
                movieDir = await m_mediaBaseRequest.GetMovieDirectoryByIDAsync(id);
                PhysicalFileResult file = PhysicalFile(movieDir, "application/x-mpegURL", true);
                return file;
            }
            else
            {
                movieDir = @"C:/MediaServer/Movies/Guardians of the Galaxy Vol. 2/" + id;
                PhysicalFileResult file = PhysicalFile(movieDir, "application/x-mpegURL", true);
                return file;
            }

        }

        [Route("media/get/movies/{name}")]
        public async IAsyncEnumerable<string> GetMoviesByNameAsync(string name)
        {
            List<MediaBase> movies = await m_mediaBaseRequest.GetMoviesByNameAsync(name);
            foreach (MediaBase media in movies)
            {
                bool movieExists = await m_mediaBaseRequest.MovieExistsAsync(media.ID);
                media.IsDownloaded = movieExists;
                yield return JsonConvert.SerializeObject(media);
            }
        }

        [Route("media/get/movie/{id}")]
        public async Task<ActionResult<string>> GetMovieByIDAsync(string id)
        {
            MediaBase mediaBase = await m_mediaBaseRequest.GetMovieByIDAsync(id);
            bool movieExists = await m_mediaBaseRequest.MovieExistsAsync(id);
            mediaBase.IsDownloaded = movieExists;
            return JsonConvert.SerializeObject(mediaBase);
        }
    }
}
