using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.MediaPipeline
{
    public class MoviePipeline : IMediaPipeline
    {
        private IEnumerable<IMediaService> m_services;

        public MoviePipeline(IEnumerable<IMediaService> services)
        {
            m_services = services;
            foreach (IMediaService service in m_services)
            {
                Console.WriteLine($"Added Service: {service.GetType().Name}");
            }
        }

        public async Task ProcessMediaAsync(MediaBase media)
        {
            foreach (IMediaService service in m_services)
            {
                await service.ProcessMediaAsync(media);
            }
        }
    }
}
