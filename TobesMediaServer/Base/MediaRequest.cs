using System;
using System.Linq;
using System.Collections.Generic;
using System.Diagnostics;
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
    public class MediaRequest : ServerRequest
    {
        private NZBgetManager m_nzbgetManager = new NZBgetManager();
        private OmdbManager m_omdbManager = new OmdbManager();
        private NzbGeekManager m_nzbGeekManager = new NzbGeekManager();
        private MovieDatabase m_movieDatabase = new MovieDatabase();
        private DownloadDatabase m_downloadDatabase = new DownloadDatabase();
        private VideoConverter m_videoConverter = new VideoConverter();

        private System.Timers.Timer m_timer = new System.Timers.Timer();
        private Dictionary<int, DownloadItem> m_currentDownloads = new Dictionary<int, DownloadItem>();
        private string m_rootDirectory = "C:/MediaServer/Movies/";

        public MediaRequest()
        {
            m_timer.Elapsed += UpdateData;
            m_timer.Interval = 1000; // in miliseconds
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
        }

        public async Task<string> GetMovieDirectoryByIDAsync(string id)
        {
            string dir = await m_movieDatabase.GetMovieDirectoryAsync(id);
            return dir;
        }

        public async Task<bool> MovieExistsAsync(string id)
        {
            return await m_movieDatabase.MovieExistsAsync(id);
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private async void UpdateData(object sender, ElapsedEventArgs e)
        {
            List<DownloadItem> downloadItems = await m_nzbgetManager.GetDownloadItemsAsync();
            foreach (DownloadItem item in downloadItems)
            {
                if (m_currentDownloads.ContainsKey(item.ID))
                {
                    m_currentDownloads[item.ID] = item;
                    Console.Clear();
                    Console.WriteLine(item.FileName + ": " + item.Progress + "%");
                    if (item.IsCompleted)
                    {
                        m_currentDownloads.Remove(item.ID);
                        string intFilePath = FindMediaFileRecursive(item.Directory);
                        intFilePath = intFilePath.Replace('\\', '/');

                        string imdbID = m_downloadDatabase.GetImdbID(item.ID);
                        MediaBase movie = await GetMovieByIDAsync(imdbID);

                        string newDir = m_rootDirectory + FixDirectory(movie.Name);
                        Directory.CreateDirectory(newDir);

                        string newFilePath = newDir + "/" + FixDirectory(movie.Name) + Path.GetExtension(intFilePath);
                        File.Move(intFilePath, newFilePath);

                        //string convertedFilePath = await m_videoConverter.ConvertToMp4Async(newFilePath);

                        movie.IsDownloaded = true;
                        m_movieDatabase.AddMovie(movie.Name, movie.ID, newFilePath);
                        m_downloadDatabase.RemoveDownload(item.ID);

                        //THIS IS FOR TESTING
                        string movieFileDir = await m_movieDatabase.GetMovieDirectoryAsync(movie.ID);
                        ProcessStartInfo file = new ProcessStartInfo();
                        file.FileName = movieFileDir;
                        file.UseShellExecute = true;
                        Process.Start(file);
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

        public async Task PlayMovieByIDAsync(string imdbID)
        {
            string fileDir = await m_movieDatabase.GetMovieDirectoryAsync(imdbID);
            ProcessStartInfo file = new ProcessStartInfo();
            file.FileName = fileDir;
            file.UseShellExecute = true;
            Process.Start(file);
        }

        public async Task DownloadMovieByIDAsync(string imdbID)
        {
            string nzbLink = await m_nzbGeekManager.GetLinkByNzbIdAsync(imdbID);
            int id = m_nzbgetManager.DownloadMovieByNzbLink(nzbLink);
            m_currentDownloads.Add(id, new DownloadItem());
            MediaBase movie = await GetMovieByIDAsync("tt" + imdbID);
            m_downloadDatabase.AddDownload(id, imdbID);
        }

        public async Task<MediaBase> GetMovieByIDAsync(string imdbID)
        {
            MediaBase movie = await m_omdbManager.GetMovieByIDAsync(imdbID);
            return movie;
        }

        public async Task<List<MediaBase>> GetMoviesByNameAsync(string name)
        {
            return await m_omdbManager.GetMoviesByNameAsync(name);
        }
    }
}
