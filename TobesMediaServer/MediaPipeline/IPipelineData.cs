using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.MediaPipeline
{
    public interface IPipelineData
    {
        public MediaStatus? GetStatus(string id);
        public void AddMedia(MediaFile media);
    }
}
