using Microsoft.VisualStudio.Web.CodeGeneration.Contracts.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.MediaPipeline
{
    public interface IMediaPipeline
    {
        public Task ProcessMediaAsync(MediaBase media, bool restore = false);
    }
}
