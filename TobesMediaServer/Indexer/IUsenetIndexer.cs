using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.Indexer
{
    public interface IUsenetIndexer
    {
        public List<string> BannedLinks { get; }
        public Task<string> GetShowLinkByNzbIdAsync(string id, int season, int episode, int downloadIndex);
        public Task<bool> DoesShowExistAsync(string id);

    }
}
