using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.Base
{
    public class DownloadItem
    {
        public int ID { get; set; }
        public int Progress { get; set; }
        public float Speed { get; set; }
        public float FileSize { get; set; }
        public float RemainingSize { get; set; }
        public bool IsCompleted { get; set; }
        public string Directory { get; set; }
        public string FileName { get; set; }
    }
}
