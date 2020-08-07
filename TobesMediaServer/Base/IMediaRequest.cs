using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.Base
{
    public interface IMediaRequest
    {
        public Task DownloadMovieByIDAsync(MediaBase media);
        public int GetProgress(string ID);
        public bool IsDownloading(string ID);
    }
}
