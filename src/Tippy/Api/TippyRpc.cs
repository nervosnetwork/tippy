using System;
using System.Collections.Generic;
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
    [Route("api")]
    [ApiController]
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class TippyRpc : ControllerBase
    {
        readonly HashSet<string> methods = new() { "create_chain", "start_chain", "stop_chain", "mine_blocks", "revert_blocks"/*, "send_transaction"*/ };

        [HttpPost]
        public ActionResult Index()
        {
            using StreamReader requestReader = new(Request.Body);
            var requestBody = requestReader.ReadToEnd();
            var request = JsonSerializer.Deserialize<RequestObject>(requestBody);
            if (request == null)
            {
                return Ok(ResponseObject.Error());
            }

            Project? activeProject = HttpContext.Items["ActiveProject"] as Project;

            // Tippy APIs
            if (methods.Contains(request.Method))
            {
                return Ok(new ApiHandler(request, activeProject).Handle());
            }

            // Pass through to CKB RPC
            if (activeProject == null || !ProcessManager.IsRunning(activeProject))
            {
                return Ok(ResponseObject.Error(request.Id, "Start a chain first to call JSON-RPC api"));
            }
            var ckbRpcClient = RawRpcClient.GetClient(activeProject)!;
            return Ok(ckbRpcClient.Call(request));
        }
    }

    class ApiHandler
    {
        readonly RequestObject request;
        readonly Project? project;

        internal ApiHandler(RequestObject request, Project? project)
        {
            this.request = request;
            this.project = project;
        }

        internal string Handle()
        {
            // TODO: dispatch and run api
            // TODO: capture `send_transaction`, save and re-send to CKB RPC
            return "TODO";
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

    class ResponseObject
    {
        internal static object Error(object? id = null, string? error = null)
        {
            return new
            {
                jsonrpc = "2.0",
                id,
                error = error ?? "Bad JSON-RPC Request",
            };
        }
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

        internal static RawRpcClient? GetClient(Project? project)
        {
            if (project != null)
            {
                return new RawRpcClient($"http://localhost:{project.NodeRpcPort}");
            }

            return null;
        }
    }
}
