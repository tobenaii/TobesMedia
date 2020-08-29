using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Net;
using Newtonsoft.Json;
using TobesMediaCore.Network;
using Newtonsoft.Json.Linq;
using System.Net.Http;
using TobesMediaServer.MediaRequest;
using TobesMediaServer.NZBManager;
using TobesMediaCore.MediaRequest;
using System.Timers;
using System.Linq;

namespace TobesMediaServer.NZBGet
{
    public class NZBgetManager : INzbManager
    {
        private HttpClient m_client;
        private Timer m_timer = new Timer();
        private List<DownloadItem> m_downloadItems;

        List<DownloadItem> INzbManager.DownloadItems => m_downloadItems;

        public NZBgetManager()
        {
            m_client = new HttpClient();
            
            m_timer.Elapsed += UpdateData;
            m_timer.Interval = 1000; // in miliseconds
            m_timer.AutoReset = true;
            m_timer.Enabled = true;
        }

        private async void UpdateData(object sender, ElapsedEventArgs e)
        {
            UpdateItemsAsync();
        }

        private async Task UpdateItemsAsync()
        {
            m_timer.Stop();
            var webClient = new WebClient();
            RPCRequest request = new RPCRequest();
            request.jsonrpc = "2.0";
            request.id = 1;
            request.method = "listgroups";
            request.parms = new object[]
                {
                        0,
                };
            try
            {
                string queueItemsJson = await webClient.UploadStringTaskAsync(new Uri("http://127.0.0.1:6789/jsonrpc/listgroups"), JsonConvert.SerializeObject(request));

                request.method = "history";
                request.parms = new object[]
                    {
                    false,
                    };
                string historyItemsJson = await webClient.UploadStringTaskAsync(new Uri("http://127.0.0.1:6789/jsonrpc/listgroups"), JsonConvert.SerializeObject(request));
                List<DownloadItem> downloadItems = ParseQueueItems(queueItemsJson);
                downloadItems.AddRange(ParseHistoryItems(historyItemsJson));
                m_downloadItems = downloadItems;
            }
            catch
            {
                m_downloadItems = null;
            }
            m_timer.Start();
        }

        public int DownloadMovieByNzbLink(string link)
        {
            RPCRequest request = new RPCRequest();
            request.jsonrpc = "2.0";
            request.id = 1;
            request.method = "append";
            request.parms = new object[]
                {
                    "",
                    link,
                    "",
                    0,
                    true,
                    false,
                    "",
                    0,
                    "SCORE",
                    new object[]{new PPParameter(){Name = "Unpack", Value = "True" }}
                };
            var jsonRequest = JsonConvert.SerializeObject(request);
            jsonRequest = jsonRequest.Replace("parms", "params");
            int id = 0;
            using (var webClient = new WebClient())
            {
                JObject jsonObj = JObject.Parse(webClient.UploadString("http://127.0.0.1:6789/jsonrpc/append", jsonRequest));
                id = Convert.ToInt32(jsonObj["result"]);
            }
            return id;
        }

        private List<DownloadItem> ParseQueueItems(string json)
        {
            JObject response = JObject.Parse(json);
            JArray downloads = JArray.Parse(response["result"].ToString());
            List<DownloadItem> downloadItems = new List<DownloadItem>();
            List<NzbgetQueueItem> queueItems = JsonConvert.DeserializeObject<List<NzbgetQueueItem>>(downloads.ToString());
            
            foreach (NzbgetQueueItem item in queueItems)
            {
                DownloadItem downloadItem = new DownloadItem();
                downloadItem.ID = item.NzbId;
                downloadItem.FileName = item.NzbName;
                downloadItem.FileSize = item.FileSizeMB;
                downloadItem.RemainingSize = item.FileSizeMB - item.DownloadedSizeMB;
                downloadItem.Progress = (int)((item.FileSizeMB == 0) ? 0 : (item.DownloadedSizeMB / item.FileSizeMB * 100.0f));
                downloadItems.Add(downloadItem);
            }
            return downloadItems;
        }

        private List<DownloadItem> ParseHistoryItems(string json)
        {
            JObject response = JObject.Parse(json);
            JArray downloads = JArray.Parse(response["result"].ToString());
            List<DownloadItem> downloadItems = new List<DownloadItem>();
            List<NzbgetHistoryItem> queueItems = JsonConvert.DeserializeObject<List<NzbgetHistoryItem>>(downloads.ToString());

            foreach (NzbgetHistoryItem item in queueItems)
            {
                DownloadItem downloadItem = new DownloadItem();
                downloadItem.ID = item.Id;
                downloadItem.FileName = item.Name;
                downloadItem.FileSize = item.FileSizeMB;
                downloadItem.Progress = 100;
                downloadItem.IsCompleted = item.MoveStatus == "SUCCESS";
                downloadItem.Directory = item.FinalDir == string.Empty ? item.DestDir : item.FinalDir;
                downloadItem.Failed = item.Status.Contains("FAILURE");
                downloadItem.IsCopy = item.Status.Contains("COPY");
                downloadItems.Add(downloadItem);
            }
            return downloadItems;
        }

        public async Task<bool> ContainsIdAsync(int id)
        {
            await UpdateItemsAsync();
            return (m_downloadItems.FirstOrDefault(x => x.ID == id) != null);
        }
    }

}

