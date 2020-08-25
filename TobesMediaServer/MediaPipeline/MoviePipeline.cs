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
        private IMediaPipelineDatabase m_database;
        private IPipelineData m_data;
        private ILocalMediaDatabase m_localDatabase;

        public MoviePipeline(IEnumerable<IMediaService> services, IMediaPipelineDatabase database, IPipelineData data, ILocalMediaDatabase local)
        {
            m_services = services;
            m_database = database;
            m_data = data;
            m_localDatabase = local;
            foreach (IMediaService service in m_services)
            {
                Console.WriteLine($"Added Service: {service.GetType().Name}");
            }
        }

        public async Task ProcessMediaAsync(MediaBase media)
        {
            MediaFile file = new MediaFile(media);
            file.IsProcessing = true;
            m_data.AddMedia(file);
            if (!await m_database.MediaExistsAsync(media.ID))
                m_database.AddMedia(media.ID);
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
            file.IsProcessing = false;
            m_database.RemoveMedia(media.ID);
            m_localDatabase.AddMedia(media.ID, file.FilePath);
        }
    }
}
