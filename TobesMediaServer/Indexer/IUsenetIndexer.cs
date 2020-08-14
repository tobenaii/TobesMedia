using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Indexer
{
    public interface IUsenetIndexer
    {
        public Task<string> GetMovieLinkByNzbIdAsync(string id);
        public Task<string> GetShowLinkByNzbIdAsync(string id);

    }
}
