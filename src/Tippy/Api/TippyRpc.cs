using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Data;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;
using Tippy.Util;

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

        readonly HashSet<string> methods = new()
        {
            "create_chain",
            "delete_chain",
            "list_chains",
            "set_active_chain",
            "start_chain",
            "stop_chain",
            "start_miner",
            "stop_miner",
            "mine_blocks",
            "revert_blocks",
            "ban_transaction",
            "unban_transaction",
        };

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
                    var result = new ApiHandler(dbContext, request, activeProject).Handle();
                    return Ok(ResponseObject.Result(result.Result, request.Id));
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
        readonly TippyDbContext dbContext;
        readonly RequestObject request;
        readonly Project? project;

        internal ApiHandler(TippyDbContext dbContext, RequestObject request, Project? project)
        {
            this.dbContext = dbContext;
            this.request = request;
            this.project = project;
        }

        internal async Task<object> Handle()
        {
            // Dispatch and run api
            return request.Method switch
            {
                "create_chain" => await CreateChain(),
                "delete_chain" => await DeleteChain(),
                "list_chains" => await ListChains(),
                "set_active_chain" => await SetActiveChain(),
                "start_chain" => StartChain(),
                "stop_chain" => StopChain(),
                "start_miner" => StartMiner(),
                "stop_miner" => StopMiner(),
                "mine_blocks" => MineBlocks(),
                "revert_blocks" => RevertBlocks(),
                "ban_transaction" => await BanTransaction(),
                "unban_transaction" => await UnbanTransaction(),
                // TODO: other apis
                _ => "TODO",
            };
        }

        // Create a dev chain and set as active chain
        async Task<object> CreateChain()
        {
            CreateChainParam? param = null;
            if (request.Params != null &&　request.Params.Length == 1)
            {
                try
                {
                    param = JsonSerializer.Deserialize<CreateChainParam>(request.Params[0].ToString()!);
                }
                catch
                {
                    param = null;
                }
            }

            var projects = await dbContext.Projects.ToListAsync();
            var calculatingFromUsed = projects.Count > 0;
            var rpcPorts = projects.Select(p => p.NodeRpcPort);
            var networkPorts = projects.Select(p => p.NodeNetworkPort);
            var indexerPorts = projects.Select(p => p.IndexerRpcPort);
            var toml = param?.GenesisIssuedCells.Select((c) => c.ToTomlString()) ?? Array.Empty<string>();

            var project = new Project
            {
                Name = $"CKB devchain",
                Chain = Project.ChainType.Dev,
                NodeRpcPort = calculatingFromUsed ? rpcPorts.Max() + 3 : 8114,
                NodeNetworkPort = calculatingFromUsed ? networkPorts.Max() + 3 : 8115,
                IndexerRpcPort = calculatingFromUsed ? indexerPorts.Max() + 3 : 8116,
                LockArg = param?.AssemblerLockArg ?? "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7",
                ExtraToml = String.Join("\n\n", toml)
            };
            projects.ForEach(p => p.IsActive = false);
            project.IsActive = true;
            dbContext.Projects.Add(project);
            await dbContext.SaveChangesAsync();

            return new 
            {
                id = project.Id,
                name = project.Name
            };
        }

        // Delete a chain
        async Task<object> DeleteChain()
        {
            int? id = null;
            if (request.Params != null && request.Params.Length == 1)
            {
                try
                {
                    id = int.Parse(request.Params[0].ToString()!);
                }
                catch
                {
                }
            }

            if (id != null)
            {
                var project = await dbContext.Projects.FindAsync(id);
                if (project != null)
                {
                    ProcessManager.ResetData(project);
                    dbContext.Projects.Remove(project);
                    await dbContext.SaveChangesAsync();
                }
            }

            return "ok";
        }

        // List all chains
        async Task<object> ListChains()
        {
            var projects = await dbContext.Projects.ToListAsync();
            return projects.Select(p =>
            {
                return new
                {
                    id = p.Id,
                    name = p.Name,
                    chain_type = p.Chain.ToString().ToLower(),
                    is_active = p.IsActive,
                };
            });
        }

        // Set active chain
        async Task<object> SetActiveChain()
        {
            int? id = null;
            if (request.Params != null && request.Params.Length == 1)
            {
                try
                {
                    id = int.Parse(request.Params[0].ToString()!);
                }
                catch
                {
                }
            }

            if (id != null)
            {
                var project = await dbContext.Projects.FindAsync(id);
                if (project != null)
                {
                    var projects = await dbContext.Projects.ToListAsync();
                    projects.ForEach(p => p.IsActive = false);
                    project.IsActive = true;
                    await dbContext.SaveChangesAsync();
                }
            }

            return "ok";
        }

        // Start the active chain
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

        // Stop the running active chain
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

        // Start the default miner
        object StartMiner()
        {
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (!ProcessManager.IsRunning(project))
            {
                throw new Exception("Active chain is not running.");
            }

            ProcessManager.StartMiner(project, ProcessManager.MinerMode.Default);
            return "ok";
        }

        // Stop the running default miner
        object StopMiner()
        {
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (!ProcessManager.IsRunning(project))
            {
                throw new Exception("Active chain is not running.");
            }

            ProcessManager.StopMiner(project);
            return "ok";
        }

        // Mine N blocks at the default 1sec interval.
        object MineBlocks()
        { 
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (!ProcessManager.IsRunning(project))
            {
                throw new Exception("Active chain is not running.");
            }

            if (request.Params == null || request.Params.Length != 1)
            {
                throw new Exception("Must provide a param as number of blocks to mine.");
            }

            dbContext.Entry(project)
                .Collection(p => p.DeniedTransactions)
                .Load();

            try
            {
                var number = int.Parse(request.Params[0].ToString()!);
                ProcessManager.StartMiner(project, ProcessManager.MinerMode.Sophisticated, number, 1);
            }
            catch (System.InvalidOperationException e)
            {
                throw new Exception(e.Message);
            }

            return $"Wait for blocks to be mined.";
        }

        // Revert N blocks.
        object RevertBlocks()
        { 
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (!ProcessManager.IsRunning(project))
            {
                throw new Exception("Active chain is not running.");
            }

            if (request.Params == null || request.Params.Length != 1)
            {
                throw new Exception("Must provide a param as number of blocks to mine.");
            }

            var client = new Ckb.Rpc.Client($"http://localhost:{project.NodeRpcPort}");

            try
            {
                var number = ulong.Parse(request.Params[0].ToString()!);
                var tipNumber = client.GetTipBlockNumber();
                var targetNumber = (tipNumber - number);
                if (number > tipNumber)
                {
                    targetNumber = 0;
                }
                var targetBlock = client.GetBlockByNumber(targetNumber)!;
                client.Truncate(targetBlock.Header.Hash);
            }
            catch (System.InvalidOperationException e)
            {
                throw new Exception(e.Message);
            }

            return "Reverted blocks.";
        }

        // Add a transaction to denylist
        async Task<object> BanTransaction()
        {
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (request.Params == null || request.Params.Length != 2)
            {
                throw new Exception("Must provide params as tx hash and deny type.");
            }

            var txHash = request.Params[0].ToString()!;
            var type = request.Params[1].ToString()!;
            try
            {
                var item = new DeniedTransaction
                {
                    ProjectId = project.Id,
                    TxHash = txHash,
                    DenyType = type == "propose" ? DeniedTransaction.Type.Propose : DeniedTransaction.Type.Commit
                };
                dbContext.DeniedTransactions.Add(item);
                await dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "Added to denylist.";
        }

        // Remove a transaction from denylist
        async Task<object> UnbanTransaction()
        {
            if (project == null)
            {
                throw new Exception("No active chain. Create a chain first.");
            }

            if (request.Params == null || request.Params.Length != 2)
            {
                throw new Exception("Must provide params as tx hash and deny type.");
            }

            var txHash = request.Params[0].ToString()!;
            var type = request.Params[1].ToString()!;
            try
            {
                var denyType = type == "propose" ? DeniedTransaction.Type.Propose : DeniedTransaction.Type.Commit;
                var item = dbContext
                    .DeniedTransactions
                    .Where(t => t.TxHash == txHash && t.DenyType == denyType)
                    .First();
                if (item != null)
                {
                    dbContext.DeniedTransactions.Remove(item);
                    await dbContext.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            return "Removed from denylist.";
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

    class LockScript
    {
        [JsonPropertyName("args")]
        public string Args { get; set; } = "";

        [JsonPropertyName("code_hash")]
        public string CodeHash { get; set; } = "";

        [JsonPropertyName("hash_type")]
        public string HashType { get; set; } = "";
    }

    class GenesisIssuedCell
    {
        [JsonPropertyName("capacity")]
        public string Capacity { get; set; } = default!;

        [JsonPropertyName("lock")]
        public LockScript Lock { get; set; } = default!;

        /*
        [[genesis.issued_cells]]
        capacity = 5_198_735_037_00000000
        lock.code_hash = "0x9bd7e06f3ecf4be0f2fcd2188b23f1b9fcc88e5d4b65a8637b17723bbda3cce8"
        lock.args = "0x470dcdc5e44064909650113a274b3b36aecb6dc7"
        lock.hash_type = "type"
        */
        internal string ToTomlString()
        {
            var lines = new string[]
            {
                "[[genesis.issued_cells]]",
                $"capacity = {Hex.HexToUInt64(Capacity)}",
                $"lock.code_hash = \"{Lock.CodeHash}\"",
                $"lock.args = \"{Lock.Args}\"",
                $"lock.hash_type = \"{Lock.HashType}\"",
            };
            return String.Join("\n", lines);
        }
    }

    class CreateChainParam
    {
        [JsonPropertyName("assembler_lock_arg")]
        public string? AssemblerLockArg { get; set; }

        [JsonPropertyName("genesis_issued_cells")]
        public List<GenesisIssuedCell> GenesisIssuedCells { get; set; } = new List<GenesisIssuedCell>();
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
                object raw = "";
                if (request.Params != null && request.Params.Length > 0)
                {
                    raw = request.Params[0];
                }
                var tx = new FailedTransaction
                {
                    ProjectId = project.Id,
                    RawTransaction = JsonSerializer.Serialize(raw),
                    Error = error.Error.Message,
                    CreatedAt = DateTime.Now
                };

                dbContext.FailedTransactions.Add(tx);
                await dbContext.SaveChangesAsync();
            }
        }
    }
}
