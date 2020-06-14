using FFmpeg.NET;
using FFmpeg.NET.Events;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace TobesMediaServer.ffmpeg
{
    public class VideoConverter
    {
        Engine ffmpeg;
        private static string m_binPath = "ffmpeg/binaries/ffmpeg.exe";
        public VideoConverter()
        {
            ffmpeg = new Engine(m_binPath);
            ffmpeg.Progress += OnProgress;
        }

        public async Task<string> ConvertToMp4Async(string filePath)
        {
            if (Path.GetExtension(filePath) == ".mp4")
                return filePath;
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
            Console.WriteLine("[{0} => {1}]", e.Input.FileInfo.Name, e.Output.FileInfo.Name);
            Console.WriteLine("Bitrate: {0}", e.Bitrate);
            Console.WriteLine("Fps: {0}", e.Fps);
            Console.WriteLine("Frame: {0}", e.Frame);
            Console.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Console.WriteLine("Size: {0} kb", e.SizeKb);
            Console.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }
    }
}
