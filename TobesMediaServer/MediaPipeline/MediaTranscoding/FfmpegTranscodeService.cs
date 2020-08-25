using FFmpeg.NET;
using FFmpeg.NET.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;
using TobesMediaServer.MediaPipeline;

namespace TobesMediaServer.ffmpeg
{
    public class FfmpegTranscodeService : IMediaService
    {
        private MediaPipeline.MediaFile m_mediaFile;
        Engine ffmpeg;
        private static string m_binPath = "MediaPipeline/MediaTranscoding/binaries/ffmpeg.exe";
        public FfmpegTranscodeService()
        {
            ffmpeg = new Engine(m_binPath);
            ffmpeg.Progress += OnProgress;
            ffmpeg.Complete += OnComplete;
        }

        private void OnComplete(object sender, ConversionCompleteEventArgs e)
        {
            m_mediaFile.FinishedProcessing();
        }

        public async Task ProcessMediaAsync(MediaPipeline.MediaFile media, MediaType type)
        {
            string filePath = media.FilePath;
            if (Path.GetExtension(filePath) == ".mp4")
                return;
            Console.WriteLine("Processing Transcode");
            m_mediaFile = media;
            media.Message = "Transcoding";
            string newFilePath = Path.ChangeExtension(filePath, ".mp4");
            var inputFile = new FFmpeg.NET.MediaFile(filePath);
            var outputFile = new FFmpeg.NET.MediaFile(newFilePath);

            var options = new ConversionOptions();
            Console.WriteLine("Transcoding");
            await ffmpeg.ConvertAsync(inputFile, outputFile);
            File.Delete(filePath);
            media.FilePath = newFilePath;
        }

        private void OnProgress(object sender, ConversionProgressEventArgs e)
        {
            if (m_mediaFile != null)
            {
                Console.Clear();
                m_mediaFile.Progress = (int)((float)(e.ProcessedDuration.TotalSeconds / e.TotalDuration.TotalSeconds) * 100.0f);
                Console.WriteLine($"Transcoding {m_mediaFile.Media.Name}: {m_mediaFile.Progress}");
            }
        }
    }
}
