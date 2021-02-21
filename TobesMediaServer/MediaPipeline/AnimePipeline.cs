using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Database;
using TobesMediaServer.MediaInfo;

namespace TobesMediaServer.MediaPipeline
{
    public class AnimePipeline : IMediaPipeline
    {
        private IEnumerable<IMediaService> m_services;
        private IMediaPipelineDatabase m_database;
        private IPipelineData m_data;
        private ILocalMediaDatabase m_localDatabase;

        public AnimePipeline(IEnumerable<IMediaService> services, IMediaPipelineDatabase database, IPipelineData data, ILocalMediaDatabase local)
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

        public async Task ProcessMediaAsync(MediaBase media, int season, int episode, bool restore = false)
        {
            MediaFile file = new MediaFile(media, season, episode);
            file.IsProcessing = true;
            m_data.AddMedia(file);
            if (!await m_database.MediaExistsAsync(media.SearchID))
                m_database.AddMedia(media.SearchID);
            foreach (IMediaService service in m_services)
            {
                await service.ProcessMediaAsync(file, restore);
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
            file.Complete();
            file.IsProcessing = false;
            m_database.RemoveMedia(media.SearchID);
            m_localDatabase.AddMedia(media.SearchID, file.FilePath);
            var p = new Process();
            p.StartInfo = new ProcessStartInfo(file.FilePath)
            {
                UseShellExecute = true
            };
            p.Start();
        }
    }
}
