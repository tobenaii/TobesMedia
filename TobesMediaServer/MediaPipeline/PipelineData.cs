using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.MediaPipeline
{
    public class PipelineData : IPipelineData
    {
        private List<MediaFile> m_media = new List<MediaFile>();
        private Dictionary<IMediaService, string> m_logs = new Dictionary<IMediaService, string>();

        public void AddMedia(MediaFile media)
        {
            m_media.Add(media);
        }

        public MediaStatus? GetStatus(string id)
        {
            return m_media.SingleOrDefault(x => x.Media.ID == id)?.Status;
        }
    }
}
