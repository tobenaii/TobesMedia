using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.NZBGet
{
    public class NzbgetHistoryItem
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        public int FileSizeMB { get; set; }
        public string ParStatus { get; set; }
        public string UnpackStatus { get; set; }
        public string MoveStatus { get; set; }
        public string ScriptStatus { get; set; }
        public string DeleteStatus { get; set; }
        public string MarkStatus { get; set; }
        public string DestDir { get; set; }
        public string FinalDir { get; set; }
        public string Status { get; set; }
    }
}
