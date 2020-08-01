using System;
using System.Collections.Generic;
using System.Text;

namespace TobesMediaCore.Network
{
    public class ServerRequest
    {
        public int progress { get; private set; }
        public bool IsDone { get { return progress == 100; } }
        public bool IsTranscoding { get; private set; }
    }
}
