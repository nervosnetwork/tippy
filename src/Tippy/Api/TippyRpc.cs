using System;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Ckb.Rpc;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;

namespace Tippy.Api
{

    // TODO:
    //    1. Add tippy api
    //    2. Capture send tx rpc method
    //    3. Pass other to CKB rpc
    [Route("api")]
    [ApiController]
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class TippyRpc : ControllerBase
    {
        [HttpPost]
        public ActionResult Index()
        {
            using StreamReader requestReader = new(Request.Body);
            var requestBody = requestReader.ReadToEnd();
            var request = JsonSerializer.Deserialize<RequestObject>(requestBody);
            if (request == null)
            {
                return Ok(RpcError());
            }

            // TODO: apis that don't require ckb node to be running

            var ckbRpcClient = CkbRpcClient();
            if (ckbRpcClient == null)
            {
                return Ok(RpcError(request.Id, "Start a chain first to call JSON-RPC api"));
            }

            // TODO: apis that require dkb node to be running

            // Pass through to CKB RPC
            return Ok(ckbRpcClient.Call(request));
        }


        static object RpcError(object? id = null, string? error = null)
        {
            return new
            {
                jsonrpc = "2.0",
                id,
                error = error ?? "Bad JSON-RPC Request",
            };
        }

        RawRpcClient? CkbRpcClient()
        {
            Project? activeProject = CurrentRunningProject();
            if (activeProject != null)
            {
                return new RawRpcClient($"http://localhost:{activeProject.NodeRpcPort}");
            }

            return null;
        }

        Project? CurrentRunningProject()
        {
            if (HttpContext.Items["ActiveProject"] is Project activeProject && ProcessManager.IsRunning(activeProject))
            {
                return activeProject;
            }
            return null;
        }
    }

    class RequestObject
    {
        [JsonPropertyName("id")]
        public object? Id { get; set; } = "1";

        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; } = "2.0";

        [JsonPropertyName("method")]
        public string Method { get; set; } = default!;

        [JsonPropertyName("params")]
        public object[]? Params { get; set; }
    }

    class RawRpcClient
    {
        readonly string Url;
        internal RawRpcClient(string url)
        {
            this.Url = url;
        }

        internal string Call(RequestObject request)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;

            var serialized = JsonSerializer.Serialize(request);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            webRequest.ContentLength = bytes.Length;
            using Stream body = webRequest.GetRequestStream();
            body.Write(bytes, 0, bytes.Length);

            using WebResponse webResponse = webRequest.GetResponse();
            using Stream responseStream = webResponse.GetResponseStream();
            using StreamReader responseReader = new(responseStream);
            return responseReader.ReadToEnd();
        }
    }
}
