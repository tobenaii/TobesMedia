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
using TobesMediaServer.MediaRequest;
using TobesMediaServer.Database;
using TobesMediaServer.ffmpeg;
using TobesMediaServer.NZBGet;
using TobesMediaServer.OMDB;

namespace TobesMediaServer.Controllers
{
    [Route("api/")]
    [ApiController]
    public class MovieController : ControllerBase
    {
        //private IMovieRequest m_movieRequest;
        //private static VideoConverter m_videoConverter = new VideoConverter();
        //private static MySqlMovieDatabase m_movieDatabase = new MySqlMovieDatabase();

        //public MovieController(IMovieRequest mediaRequest)
        //{
        //    m_movieRequest = mediaRequest;
        //}

        //[Route("media/play/movie/{id}")]
        //public async Task<FileResult> PlayMovieByIDAsync(string id)
        //{
        //    Console.WriteLine(id);
        //    string movieDir = await m_movieDatabase.GetMovieDirectoryAsync(id);
        //    PhysicalFileResult file = PhysicalFile(movieDir, "application/x-mpegURL", true);
        //    return file;
        //}

        //[Route("media/get/movie/progress/{id}")]
        //public int GetMovieProgressByID(string id)
        //{
        //    return 0;
        //    //return m_movieRequest.GetProgress(id);
        //}

        //[Route("media/get/movie/isDownloading/{id}")]
        //public bool GetMovieDownloadingByID(string id)
        //{
        //    return false;
        //    //return m_movieRequest.IsDownloading(id);
        //}
    }
}
