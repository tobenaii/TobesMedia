using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using StreamJsonRpc;
using TobesMediaCore.Network;

namespace TobesMediaServer.Controllers
{
    [Serializable]
    public struct RPCRequest
    {
        public string jsonrpc;
        public int id;
        public string method;
        public object[] parms;
    }

    public struct PPParameter
    {
        public string Name;
        public string Value;
    }

    [Route("api/[controller]")]
    [ApiController]
    public class MediaController : ControllerBase
    {
        [HttpPut("{id}")]
        public async void Put(string id)
        {
            string link = await GetNZB(id);
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
                    new object[]{new PPParameter(){Name = "Unpack", Value = "True" } }
                };
            var json = JsonConvert.SerializeObject(request);
            json = json.Replace("parms", "params");
            using (var webClient = new WebClient())
            {
                webClient.UploadString("http://127.0.0.1:6789/jsonrpc/append", json);
            }
        }

        private async Task<string> GetNZB(string imdbID)
        {
            return await new MediaBaseRequest().GetNZB(imdbID, new System.Net.Http.HttpClient());
        }
    }
}
