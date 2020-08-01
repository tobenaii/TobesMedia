using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using TobesMediaCore.Data.Media;
using TobesMediaServer.Base;
using TobesMediaServer.NZBGet;
using TobesMediaServer.OMDB;
using TobesMediaServer.NZBGeek;
using TobesMediaServer.Database;
using TobesMediaServer.ffmpeg;

namespace TobesMediaCore.Network
{
    [Serializable]
    public struct RPCRequest
    {
        public string jsonrpc;
        public int id;
        public string method;
        public object[] parms;
    }

    [Serializable]
    public struct PPParameter
    {
        public string Name;
        public string Value;
    }
    public class MediaRequest
    {
        private NZBgetManager m_nzbgetManager = new NZBgetManager();
        private NzbGeekManager m_nzbGeekManager = new NzbGeekManager();
        private DownloadDatabase m_downloadDatabase = new DownloadDatabase();

        private System.Timers.Timer m_timer = new System.Timers.Timer();
        private Dictionary<int, MediaBase> m_currentDownloads = new Dictionary<int, MediaBase>();
        private string m_rootDirectory = "C:/MediaServer/Movies/";

        public MediaRequest()
        {
            m_timer.Elapsed += UpdateData;
            m_timer.Interval = 1000; // in miliseconds
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
        }

        //public async Task<string> GetMovieDirectoryByIDAsync(string id)
        //{
        //    string dir = await m_movieDatabase.GetMovieDirectoryAsync(id);
        //    return dir;
        //}

        //public async Task<bool> MovieExistsAsync(string id)
        //{
        //    return await m_movieDatabase.MovieExistsAsync(id);
        //}

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void UpdateData(object sender, ElapsedEventArgs e)
        {
            List<DownloadItem> downloadItems = await m_nzbgetManager.GetDownloadItemsAsync();
            if (downloadItems == null)
                return;
            foreach (DownloadItem item in downloadItems)
            {
                if (m_currentDownloads.ContainsKey(item.ID))
                {
                    MediaBase media = m_currentDownloads[item.ID];
                    media.Progress = item.Progress;
                    Console.Clear();
                    Console.WriteLine(item.FileName + ": " + item.Progress + "%");
                    if (item.IsCompleted)
                    {
                        m_currentDownloads.Remove(item.ID);
                        string intFilePath = FindMediaFileRecursive(item.Directory);
                        intFilePath = intFilePath.Replace('\\', '/');

                        string imdbID = m_downloadDatabase.GetImdbID(item.ID);
                        //MediaBase movie = await GetMovieByIDAsync(imdbID);

                        string newDir = m_rootDirectory + FixDirectory(media.Name);
                        Directory.CreateDirectory(newDir);

                        string newFilePath = newDir + "/" + FixDirectory(media.Name) + Path.GetExtension(intFilePath);
                        File.Move(intFilePath, newFilePath);
                        m_downloadDatabase.RemoveDownload(item.ID);
                        media.Progress = 100;
                        //movie.IsDownloaded = true;
                        //string convertedFilePath = await m_videoConverter.ConvertToMp4Async(newFilePath);
                        //if (convertedFilePath != newFilePath)
                        //movie.IsTranscoding = true;
                        //.AddMovie(movie.Name, movie.ID, newFilePath);

                        //THIS IS FOR TESTING
                        //string movieFileDir = await m_movieDatabase.GetMovieDirectoryAsync(movie.ID);
                        //ProcessStartInfo file = new ProcessStartInfo();
                        //file.FileName = movieFileDir;
                        //file.UseShellExecute = true;
                        //Process.Start(file);
                    }
                }
            }
        }

        private string FixDirectory(string dir)
        {
            return string.Concat(dir.Split(Path.GetInvalidFileNameChars()));
        }

#pragma warning restore VSTHRD100 // Avoid async void methods

        private string[] m_videoFormats = new string[] { ".mp4", ".mkv", ".avi", ".m4a", ".m4v", ".f4v", ".wmv" };
        private string FindMediaFileRecursive(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                if (m_videoFormats.Contains(Path.GetExtension(file).ToLower()))
                    return file;
            }
            string[] directories = Directory.GetDirectories(directory);
            foreach (string nextDirectory in directories)
            {
                string file = FindMediaFileRecursive(nextDirectory);
                if (file != "")
                    return file;
            }
            return "";
        }

        public async Task DownloadMovieByIDAsync(MediaBase media)
        {
            string nzbLink = await m_nzbGeekManager.GetLinkByNzbIdAsync(media.ID);
            int id = m_nzbgetManager.DownloadMovieByNzbLink(nzbLink);
            m_currentDownloads.Add(id, media);
            m_downloadDatabase.AddDownload(id, media.ID);
        }

        public int GetProgress(string ID)
        {
            MediaBase media = m_currentDownloads.Values.FirstOrDefault(x => x.ID == ID);
            return (int)media.Progress;
        }

        public bool IsDownloading(string ID)
        {
            MediaBase media = m_currentDownloads.Values.FirstOrDefault(x => x.ID == ID);
            return media != null;
        }
    }
}
