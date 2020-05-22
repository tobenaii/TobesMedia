using System;
using System.Collections.Generic;
using System.Text;

namespace TobesMediaCore.Network
{
    public class ServerRequest
    {
        public int progress { get; protected set; }
        public bool IsDone { get { return progress == 100; } }
    }
}
