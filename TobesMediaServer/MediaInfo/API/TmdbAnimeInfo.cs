using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaServer.Indexer;

namespace TobesMediaServer.MediaInfo.API
{
    public class TmdbAnimeInfo : TmdbShowInfo, IAnimeInfo
    {
        protected override bool ShowAnime { get => true; }

        public TmdbAnimeInfo(IUsenetIndexer indexer)
            :base(indexer)
        {

        }
    }
}
