using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;
using TobesMediaServer.MediaPipeline;
using Xabe.FFmpeg;

namespace TobesMediaServer.Transcoding
{
    public class FfmpegTranscodeService : IMediaService
    {
        private MediaPipeline.MediaFile m_mediaFile;
        private const string m_binPath = "MediaPipeline/MediaTranscoding/binaries/";
        Xabe.FFmpeg.FFmpeg ffmpeg;
        private IServiceLogger m_logger;

        public FfmpegTranscodeService(IServiceLogger logger, string ffmpegPath = m_binPath)
        {
            Xabe.FFmpeg.FFmpeg.SetExecutablesPath(ffmpegPath);
            m_logger = logger;
        }

        private void OnComplete(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs e)
        {
            m_mediaFile.FinishedProcessing();
        }

        public async Task ProcessMediaAsync(MediaPipeline.MediaFile media, bool restore)
        {
            string filePath = media.FilePath;
            Console.WriteLine("Processing Transcode");
            m_mediaFile = media;
            media.Message = "Transcoding";
            string newFilePath = Path.ChangeExtension(filePath, ".webm");
            if (File.Exists(newFilePath))
                File.Delete(newFilePath);
            //await Task.Run(async () => { while (File.Exists(newFilePath)) { await Task.Delay(100); } });
            Console.WriteLine("Transcoding");
            IMediaInfo mediaInfo = await Xabe.FFmpeg.FFmpeg.GetMediaInfo(filePath);
            IStream videoStream = mediaInfo.VideoStreams.FirstOrDefault()?.SetCodec(VideoCodec.vp9);
            IStream audioStream = mediaInfo.AudioStreams.FirstOrDefault()?.SetCodec(AudioCodec.opus);
            IConversion conversion = Xabe.FFmpeg.FFmpeg.Conversions.New();
            conversion.OnProgress += OnProgress;
            await conversion.AddStream(audioStream, videoStream)
                .AddParameter("-strict -2")
                .AddParameter("-quality realtime")
                .AddParameter("-speed 6").AddParameter("-tile-columns 5")
                .AddParameter("-frame-parallel 1")
                .AddParameter("-threads 24")
                .AddParameter("-row-mt 1")
                .SetOutput(newFilePath).Start();
            File.Delete(filePath);
            media.FilePath = newFilePath;
            m_mediaFile.FinishedProcessing();
        }



        private void OnProgress(object sender, Xabe.FFmpeg.Events.ConversionProgressEventArgs e)
        {
            if (m_mediaFile != null)
            {
                m_mediaFile.Progress = (int)((float)(e.Duration.TotalSeconds / e.TotalLength.TotalSeconds) * 100.0f);
                m_logger.Log($"Transcoding {m_mediaFile.Media.Name}: {m_mediaFile.Progress}", this);
            }
        }
    }
}
