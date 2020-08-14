using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Database;
using TobesMediaServer.MediaInfo;

namespace TobesMediaServer.MediaPipeline
{
    public class MoviePipeline : IMediaPipeline
    {
        private IEnumerable<IMediaService> m_services;
        private IMediaDatabase m_database;

        public MoviePipeline(IEnumerable<IMediaService> services, IMediaDatabase database)
        {
            m_services = services;
            m_database = database;
            foreach (IMediaService service in m_services)
            {
                Console.WriteLine($"Added Service: {service.GetType().Name}");
            }
        }

        public async Task ProcessMediaAsync(MediaBase media)
        {
            MediaFile file = new MediaFile(media);
            if (!await m_database.MediaExistsAsync("Pipeline", media.ID))
                m_database.AddMedia("Pipeline", media.ID);
            foreach (IMediaService service in m_services)
            {
                await service.ProcessMediaAsync(file, MediaType.Movies);
                while (!file.IsFinishedProcessing)
                {
                    if (file.ShouldStopAllProcessing)
                    {
                        Console.WriteLine($"{service.GetType().Name} has stopped all further processing of this media");
                        return;
                    }
                    await Task.Delay(100);
                }
            }
            m_database.RemoveMedia("Pipeline", media.ID);
        }
    }
}
