using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.NZBGet
{
    public class NzbgetQueueItem
    {
        public int NzbId { get; set; }
        public string NzbName { get; set; }
        public string Category { get; set; }
        public float FileSizeMB { get; set; }
        public float DownloadedSizeMB { get; set; }
        public int ActiveDownloads { get; set; }
        public List<NzbgetParameter> Parameters { get; set; }
    }
}
