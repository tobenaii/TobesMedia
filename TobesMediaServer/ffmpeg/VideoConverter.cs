using FFmpeg.NET;
using FFmpeg.NET.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TobesMediaCore.Data.Media;

namespace TobesMediaServer.ffmpeg
{
    public class VideoConverter
    {
        private MediaBase m_mediaBase;
        Engine ffmpeg;
        private static string m_binPath = "ffmpeg/binaries/ffmpeg.exe";
        public VideoConverter()
        {
            ffmpeg = new Engine(m_binPath);
            ffmpeg.Progress += OnProgress;
        }

        public async Task<string> ConvertToMp4Async(string filePath, MediaBase media = null)
        {
            if (Path.GetExtension(filePath) == ".mp4")
                return filePath;
            m_mediaBase = media;
            string newFilePath = Path.ChangeExtension(filePath, ".mp4");
            var inputFile = new MediaFile(filePath);
            var outputFile = new MediaFile(newFilePath);

            var options = new ConversionOptions();
            await ffmpeg.ConvertAsync(inputFile, outputFile);
            File.Delete(filePath);
            return newFilePath;
        }

        private void OnProgress(object sender, ConversionProgressEventArgs e)
        {
            Console.Clear();
            if (m_mediaBase != null)
                m_mediaBase.Progress = (e.TotalDuration.Seconds / e.ProcessedDuration.Seconds) * 100.0f;
        }
    }
}
