using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Data;
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
        public TippyRpc(TippyDbContext context)
        {
            dbContext = context;
        }

        readonly TippyDbContext dbContext;

        readonly HashSet<string> methods = new() { "create_chain", "start_chain", "stop_chain", "mine_blocks", "revert_blocks" };

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
                try
                {
                    var result = new ApiHandler(request, activeProject).Handle();
                    return Ok(ResponseObject.Result(result, request.Id));
                }
                catch (Exception ex)
                {
                    return Ok(ResponseObject.Error(ex.Message, request.Id));
                }
            }

            // Pass through to CKB RPC
            if (activeProject == null || !ProcessManager.IsRunning(activeProject))
            {
                return Ok(ResponseObject.Error("Start a chain first to call JSON-RPC api.", request.Id));
            }
            var ckbRpcClient = RawRpcClient.GetClient(activeProject)!;
            var ckbResult = ckbRpcClient.Call(request);
            if (request.Method == "send_transaction")
            {
                TransactionRecorder.RecordIfNecessary(dbContext, request, ckbResult, activeProject);
            }
            return Ok(ckbResult);
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

        internal object Handle()
        {
            // Dispatch and run api
            return request.Method switch
            {
                "start_chain" => StartChain(),
                "stop_chain" => StopChain(),
                // TODO: other apis
                _ => "TODO",
            };
        }

        object StartChain()
        {
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (ProcessManager.IsRunning(project))
            {
                throw new Exception("Active chain is alredy running.");
            }

            ProcessManager.Start(project);
            return "ok";
        }

        object StopChain()
        {
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (!ProcessManager.IsRunning(project))
            {
                throw new Exception("Active chain is not running.");
            }

            ProcessManager.Stop(project);
            return "ok";
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
        internal static object Error(string? error = null, object? id = null)
        {
            return new
            {
                jsonrpc = "2.0",
                id,
                error = error ?? "Bad JSON-RPC Request",
            };
        }

        internal static object Result(object result, object? id = null)
        {
            return new
            {
                jsonrpc = "2.0",
                id,
                result,
            };
        }
    }

    class Error
    {
        [JsonPropertyName("code")]
        public int Code { get; set; } = 0;

        [JsonPropertyName("message")]
        public String Message { get; set; } = "";
    }

    class ErrorResponseObject
    {
        [JsonPropertyName("error")]
        public Error Error { get; set; } = default!;
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

    class TransactionRecorder
    {
        internal static async void RecordIfNecessary(TippyDbContext dbContext, RequestObject request, string result, Project project)
        {
            var error = JsonSerializer.Deserialize<ErrorResponseObject>(result);
            if (error != null)
            {
                var tx = new FailedTransaction
                {
                    ProjectId = project.Id,
                    RawTransaction = JsonSerializer.Serialize(request.Params),
                    Error = error.Error.Message,
                    CreatedAt = DateTime.Now
                };

                dbContext.FailedTransactions.Add(tx);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
