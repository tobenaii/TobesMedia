using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Threading;
using System.Timers;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using System.Diagnostics;
using TobesMediaCore.Network;
using Newtonsoft.Json.Linq;
using System.ComponentModel;
using System.IO;
using Microsoft.Extensions.Localization.Internal;

namespace TobesMediaServer.Base
{
    public class MediaDownload
    {
        public struct PPParameter
        {
            public string Name;
            public string Value;
        }
        public struct ServerStats
        {
            public int ServerID;
            public int SuccessArticles;
            public int FailedArticles;
        }

        public struct ScriptStatus
        {
            public string Name;
            public string Status;
        }

        private struct Data
        {
            public int NZBID;
            public string NZBFilename;
            public string NZBName;
            public string Kind;
            public string URL;
            public string DestDir;
            public string FinalDir;
            public string Category;
            public float FileSizeLo;
            public float FileSizeHi;
            public float FileSizeMB;
            public float RemainingSizeLo;
            public float RemainingSizeHi;
            public float RemainingSizeMB;
            public float PausedSizeLo;
            public float PausedSizeHi;
            public float PausedSizeMB;
            public int FileCount;
            public int RemainingFileCount;
            public int RemainingParCount;
            public int MinPostTime;
            public int MaxPostTime;
            public int MaxPriority;
            public int ActiveDownloads;
            public string Status;
            public int TotalArticles;
            public int SuccessArticles;
            public int FailedArticles;
            public int Health;
            public int CriticalHealth;
            public float DownloadedSizeLo;
            public float DownloadedSizeHi;
            public float DownloadedSizeMB;
            public int DownloadTimeSec;
            public int MessageCount;
            public string DupeKey;
            public int DupeScore;
            public string DupeMode;
            public PPParameter[] Parameters;
            public ServerStats[] ServerStats;
            public string ParStatus;
            public string UnpackStatus;
            public string MoveStatus;
            public string ScriptStatus;
            public string DeleteStatus;
            public string MarkStatus;
            public ScriptStatus[] ScriptStatuses;
            public int PostTotalTimeSec;
            public int ParTimeSec;
            public int RepairTimeSec;
            public int UnpackTimeSec;
            public string PostInfoText;
            public int PostStageProgress;
            public int PostStageTimeSec;
        }


        private Data? m_downloadData;
        private System.Timers.Timer m_timer;
        private int m_NZBID;

        public int Progress => (int)((m_downloadData.Value.FileSizeMB == 0) ? 0 : (m_downloadData.Value.DownloadedSizeMB / m_downloadData.Value.FileSizeMB * 100.0f));

        public MediaDownload(int NZBID)
        {
            m_NZBID = NZBID;
            m_timer = new System.Timers.Timer();
            m_timer.Elapsed += UpdateData;
            m_timer.Interval = 1000; // in miliseconds
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
        }

        private void UpdateData(object source, ElapsedEventArgs e)
        {
            using (var webClient = new WebClient())
            {
                RPCRequest request = new RPCRequest();
                request.jsonrpc = "2.0";
                request.id = 1;
                request.method = "listgroups";
                request.parms = new object[]
                    {
                        0,
                    };
                var jsonRequest = JsonConvert.SerializeObject(request);
                webClient.UploadStringAsync(new Uri("http://127.0.0.1:6789/jsonrpc/listgroups"), JsonConvert.SerializeObject(request));
                webClient.UploadStringCompleted += WebClient_UploadDataCompleted;
            }
        }

        private void WebClient_UploadDataCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            JObject response = JObject.Parse(e.Result);
            JArray downloads = JArray.Parse(response["result"].ToString());
            Data? newData;
            if (downloads.Count == 0)
                newData = null;
            else
                newData = JsonConvert.DeserializeObject<Data>(downloads[0].ToString());
            if (newData.HasValue)
            {
                m_downloadData = newData;
                if (m_downloadData.Value.Status == "DOWNLOADING")
                    Console.WriteLine(Progress);
                else
                    Console.WriteLine(m_downloadData.Value.Status);
            }
            else
            {
                if (Directory.Exists(m_downloadData.Value.FinalDir))
                {
                    Console.WriteLine("SUCCESS");
                    string file = FindMediaFileRecursive(m_downloadData.Value.FinalDir);
                    Console.WriteLine("Opening File: " + file);
                    Process.Start(@file);
                    m_timer.Stop();
                }
            }
        }

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
    }

}
