using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.MediaInfo
{
    public interface IMediaInfo
    {
        public Task<MediaBase> GetMediaByIDAsync(string imdbID);

        public Task<List<MediaBase>> GetMediaByNameAsync(string name);
    }
}
