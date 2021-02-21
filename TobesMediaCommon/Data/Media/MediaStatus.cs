using System;
using System.Collections.Generic;
using System.Text;

namespace TobesMediaCommon.Data.Media
{
    public struct MediaStatus
    {
        public bool IsProcessing { get; set; }
        public string Message { get; set; }
        public int Progress { get; set; }

        public bool IsComplete { get; set; }
    }
}
