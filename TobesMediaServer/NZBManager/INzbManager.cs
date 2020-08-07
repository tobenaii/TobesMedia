using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaServer.Base;

namespace TobesMediaServer.NZBManager
{
    public interface INzbManager
    {
        public int DownloadMovieByNzbLink(string link);
        public Task<List<DownloadItem>> GetDownloadItemsAsync();
    }
}
