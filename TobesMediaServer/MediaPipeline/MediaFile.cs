using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCommon.Data.Media;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.MediaPipeline
{
    public class MediaFile
    {
        public MediaBase Media { get; private set; }
        public string FilePath { get; set; }

        private MediaStatus m_status;
        public MediaStatus Status => m_status;
        public string Message { get { return Status.Message; } set { m_status.Message = value; } }
        public int Progress { get { return Status.Progress; } set { m_status.Progress = value; } }
        public bool IsProcessing { set { m_status.IsProcessing = value; } }
        public bool IsFinishedProcessing { get; private set; }
        public bool ShouldStopAllProcessing { get; private set; }

        public MediaFile(MediaBase media)
        {
            Media = media;
        }

        public void FinishedProcessing()
        {
            IsFinishedProcessing = true;
        }

        public void StopAllProcessing()
        {
            ShouldStopAllProcessing = true;
        }
    }
}
