using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using System.Timers;
using TobesMediaCore.Data.Media;
using TobesMediaServer.MediaRequest;
using TobesMediaServer.NZBGet;
using TobesMediaServer.Database;
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
        private IDownloadDatabase m_mediaDatabase;
        private IServiceLogger m_logger;
        
        private System.Timers.Timer m_timer = new System.Timers.Timer();
        private NzbMediaDownload m_mediaDownload;
        private string m_rootDirectory = "C:/MediaServer/Movies/";
        private int m_downloadIndex;
        private MediaFile m_mediaFile;
        private bool m_isRestoring;
        private string _link;

        public NzbDownloadService(INzbManager nzbManager, IUsenetIndexer indexer, IDownloadDatabase mediaDatabase, IServiceLogger logger)
        {
            m_logger = logger;
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
            {
                return;
            }
            DownloadItem item = downloadItems.FirstOrDefault(x => x.ID == m_mediaDownload.ID);
            if (item != null)
            {
                if (item.Failed || item.IsCopy)
                {
                    m_usenetIndexer.BannedLinks.Add(_link);
                    TryNextDownload();
                    return;
                }
                m_logger.Log(item.FileName + ": " + item.Progress + "%", this);
                m_mediaDownload.mediaFile.Progress = item.Progress;
                if (item.IsCompleted)
                {
                    string newDir = m_rootDirectory + FixDirectory(m_mediaDownload.mediaFile.Media.Name) + "S" + m_mediaDownload.mediaFile.Season + "E" + m_mediaDownload.mediaFile.Episode;
                    if (m_isRestoring)
                    {
                        m_mediaDownload.mediaFile.FilePath = FindMediaFileRecursive(newDir);
                        if (m_mediaDownload.mediaFile.FilePath != string.Empty)
                        {
                            m_mediaDownload.mediaFile.FinishedProcessing();
                            m_timer.Stop();
                            return;
                        }
                    }
                    string intFilePath = FindMediaFileRecursive(item.Directory);
                    intFilePath = intFilePath.Replace('\\', '/');

                    //MediaBase movie = await GetMovieByIDAsync(imdbID);
                    
                    Directory.CreateDirectory(newDir);

                    string newFilePath = newDir + "/" + FixDirectory(m_mediaDownload.mediaFile.Media.Name) + "S" + m_mediaFile.Season + "E" + m_mediaFile.Episode + Path.GetExtension(intFilePath);
                    File.Move(intFilePath, newFilePath);
                    m_mediaDownload.mediaFile.Progress = 100;
                    m_mediaDownload.mediaFile.FilePath = newFilePath;
                    m_mediaDownload.mediaFile.FinishedProcessing();
                    m_timer.Stop();
                    m_mediaDatabase.RemoveMedia(m_mediaFile.Media.SearchID, m_mediaFile.Season, m_mediaFile.Episode);
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
            if (!Directory.Exists(directory))
                return string.Empty;
            string[] files = Directory.GetFiles(directory);
            foreach (string file in files)
            {
                if (file.ToLower().Contains("sample"))
                    continue;
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
            return string.Empty;
        }

        public async Task ProcessMediaAsync(MediaFile mediaFile, bool restore)
        {
            string newDir = m_rootDirectory + FixDirectory(mediaFile.Media.Name) + "S" + mediaFile.Season + "E" + mediaFile.Episode;
            string mediaCheck = FindMediaFileRecursive(newDir);
            if (mediaCheck != string.Empty)
            {
                mediaFile.FilePath = mediaCheck;
                mediaFile.FinishedProcessing();
                m_timer.Stop();
                return;
            }
            m_isRestoring = restore;
            m_mediaFile = mediaFile;
            Console.WriteLine("Processing Download");
            string nzbLink = await m_usenetIndexer.GetShowLinkByNzbIdAsync(mediaFile.Media.SearchID, mediaFile.Season, mediaFile.Episode, m_downloadIndex);
            if (nzbLink == string.Empty)
            {
                mediaFile.StopAllProcessing();
                return;
            }
            _link = nzbLink;
            Console.WriteLine("Attempting Download");
            int id;
            string checkId = await m_mediaDatabase.GetValueAsync(mediaFile.Media.SearchID, m_mediaFile.Season, m_mediaFile.Episode);
            if (checkId != string.Empty)
                id = Convert.ToInt32(checkId);
            else
            {
                id = m_nzbManager.DownloadMovieByNzbLink(nzbLink);
                m_mediaDatabase.AddMedia(mediaFile.Media.SearchID, m_mediaFile.Season, m_mediaFile.Episode, id.ToString());
            }
            Console.WriteLine("Downloading");
            mediaFile.Message = "Downloading";
            m_mediaDownload = new NzbMediaDownload(id, mediaFile);
        }

        public void TryNextDownload()
        {
            m_downloadIndex++;
            m_mediaDatabase.RemoveMedia(m_mediaFile.Media.SearchID, m_mediaFile.Season, m_mediaFile.Episode);
            ProcessMediaAsync(m_mediaFile, m_isRestoring);
        }
    }
}
