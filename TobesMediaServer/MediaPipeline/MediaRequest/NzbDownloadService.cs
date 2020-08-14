using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using TobesMediaCore.Data.Media;
using TobesMediaServer.MediaRequest;
using TobesMediaServer.NZBGet;
using TobesMediaServer.OMDB;
using TobesMediaServer.Database;
using TobesMediaServer.ffmpeg;
using TobesMediaServer.NZBManager;
using TobesMediaServer.MediaPipeline;
using TobesMediaServer.Indexer;
using TobesMediaCommon.Data.Media;

namespace TobesMediaCore.MediaRequest
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
    public class NzbDownloadService : IMediaService
    {
        private struct NzbMediaDownload
        {
            public int ID { get; set; }
            public MediaFile mediaFile { get; set; }

            public NzbMediaDownload(int id, MediaFile file)
            {
                ID = id;
                mediaFile = file;
            }
        }

        private INzbManager m_nzbManager;
        private IUsenetIndexer m_usenetIndexer;
        private IMediaDatabase m_mediaDatabase;

        private System.Timers.Timer m_timer = new System.Timers.Timer();
        private NzbMediaDownload m_mediaDownload;
        private string m_rootDirectory = "C:/MediaServer/Movies/";

        public NzbDownloadService(INzbManager nzbManager, IUsenetIndexer indexer, IMediaDatabase mediaDatabase)
        {
            m_nzbManager = nzbManager;
            m_usenetIndexer = indexer;
            m_mediaDatabase = mediaDatabase;

            m_timer.Elapsed += UpdateData;
            m_timer.Interval = 1000; // in miliseconds
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
        }

#pragma warning disable VSTHRD100 // Avoid async void methods
        private void UpdateData(object sender, ElapsedEventArgs e)
        {
            List<DownloadItem> downloadItems = m_nzbManager.DownloadItems;
            if (downloadItems == null)
                return;
            DownloadItem item = downloadItems.FirstOrDefault(x => x.ID == m_mediaDownload.ID);
            if (item != null)
            {
                Console.Clear();
                Console.WriteLine(item.FileName + ": " + item.Progress + "%");
                if (item.IsCompleted)
                {
                    string intFilePath = FindMediaFileRecursive(item.Directory);
                    intFilePath = intFilePath.Replace('\\', '/');

                    //MediaBase movie = await GetMovieByIDAsync(imdbID);

                    string newDir = m_rootDirectory + FixDirectory(m_mediaDownload.mediaFile.Media.Name);
                    Directory.CreateDirectory(newDir);

                    string newFilePath = newDir + "/" + FixDirectory(m_mediaDownload.mediaFile.Media.Name) + Path.GetExtension(intFilePath);
                    File.Move(intFilePath, newFilePath);
                    m_mediaDownload.mediaFile.Progress = 100;
                    m_mediaDownload.mediaFile.FilePath = newFilePath;
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

        public async Task ProcessMediaAsync(MediaFile mediaFile, MediaType type)
        {
            Console.WriteLine("Processing Download");
            string nzbLink;
            if (type == MediaType.Movies)
                nzbLink = await m_usenetIndexer.GetMovieLinkByNzbIdAsync(mediaFile.Media.ID);
            else
                nzbLink = await m_usenetIndexer.GetShowLinkByNzbIdAsync(mediaFile.Media.ID);
            if (nzbLink == string.Empty)
            {
                mediaFile.StopAllProcessing();
                return;
            }
            Console.WriteLine("Attempting Download");
            int id;
            string checkId = await m_mediaDatabase.GetValueAsync("Downloads", mediaFile.Media.ID);
            if (checkId != string.Empty)
                id = Convert.ToInt32(checkId);
            else
            {
                id = m_nzbManager.DownloadMovieByNzbLink(nzbLink);
                m_mediaDatabase.AddMedia("Downloads", mediaFile.Media.ID, id.ToString());
            }
            Console.WriteLine("Downloading");
            m_mediaDownload = new NzbMediaDownload(id, mediaFile);
        }
    }
}
