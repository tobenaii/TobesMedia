using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.MediaInfo
{
    public interface IMediaInfo
    {
        public Task<MediaBase> GetMediaByIDAsync(string id);

        public Task<MediaPage> GetMediaByNameAsync(string name, int page, bool checkDownload);
    }
}
