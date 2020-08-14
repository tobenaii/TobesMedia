using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaServer.MediaRequest;

namespace TobesMediaServer.NZBManager
{
    public interface INzbManager
    {
        public int DownloadMovieByNzbLink(string link);
        public Task<bool> ContainsIdAsync(int id);
        public List<DownloadItem> DownloadItems { get; }
    }
}
