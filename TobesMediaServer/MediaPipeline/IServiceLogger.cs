using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.MediaPipeline
{
    public interface IServiceLogger
    {
        public void Log(string message, IMediaService service);
    }
}
