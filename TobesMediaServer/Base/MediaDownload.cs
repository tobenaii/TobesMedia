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

namespace TobesMediaServer.Base
{
    public class MediaDownload
    {
        private struct Data
        {
            public int ID;
            public int NZBID;
            public string NZBFileName;
            public string NZBName;
            public string Subject;
            public string Filename;
            public bool FilenameConfirmed;
            public string DestDir;
            public int FileSizeLo;
            public int FileSizeHi;
            public bool Paused;
            public int PostTime;
            public int ActiveDownloads;
            public int Progress;
        }

        [System.Serializable]
        private struct Request
        {
            public int IDFrom;
            public int IDTo;
            public int NZBID;

            public Request(int NZBID)
            {
                IDFrom = 0;
                IDTo = 0;
                this.NZBID = NZBID;
            }
        }


        private Data? m_downloadData;
        private System.Timers.Timer m_timer;
        private int m_NZBID;

        public int Progress => m_downloadData.Value.Progress;

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
                request.method = "listfiles";
                request.parms = new object[]
                    {
                        0,
                        0,
                        m_NZBID,
                    };
                var jsonRequest = JsonConvert.SerializeObject(request);
                webClient.UploadStringAsync(new Uri("http://127.0.0.1:6789/jsonrpc/listfiles"), JsonConvert.SerializeObject(request));
                webClient.UploadStringCompleted += WebClient_UploadDataCompleted;
            }
        }

        private void WebClient_UploadDataCompleted(object sender, UploadStringCompletedEventArgs e)
        {
            JObject response = JObject.Parse(e.Result);
            JArray downloads = JArray.Parse(response["result"].ToString());
            if (downloads.Count == 0)
                return;
            m_downloadData = JsonConvert.DeserializeObject<Data>(downloads[0].ToString());
            if (m_downloadData.HasValue)
                Console.WriteLine(e.Result);
        }
    }

}
