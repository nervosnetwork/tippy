using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Ckb.Rpc;
using Ckb.Rpc.Types;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.ApiData;
using System.Text.Json.Serialization;
using Tippy.Filters;
using Tippy.Util;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class BlocksController : ControllerBase
    {
        private Client? Rpc()
        {
            Project activeProject = HttpContext.Items["ActiveProject"] as Project;
            if (activeProject != null && ProcessManager.IsRunning(activeProject))
            {
                return new Client($"http://localhost:{activeProject.NodeRpcPort}");
            }
            return null;
        }

        [HttpGet]
        public ActionResult Index()
        {
            var client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            UInt64 tipBlockNumber = client.GetTipBlockNumber();
            List<UInt64> blockNumbers = GetBlockNumbers(tipBlockNumber, 10);

            BlocksResult[] brs = blockNumbers.Select(num => GetBlock(client, num)).Where(br => br != null).ToArray();
            ArrayResult<BlocksResult> result = new("block_list", brs);

            return Ok(result);
        }

        private static BlocksResult? GetBlock(Client client, UInt64 num)
        {
            Block? block = client.GetBlockByNumber(num);
            if (block == null)
            {
                return null;
            }

            var header = block.Header;
            var transactions = block.Transactions;

            int inputsCount = transactions.Select(tx => tx.Inputs.Length).Aggregate(0, (acc, cur) => acc + cur);
            int outputsCount = transactions.Select(tx => tx.Outputs.Length).Aggregate(0, (acc, cur) => acc + cur);

            var number = $"{Hex.HexToUInt64(header.Number)}";
            int transactionsCount = transactions.Length;
            string timestamp = $"{ Hex.HexToUInt64(header.Timestamp) }";

            BlocksResult br = new();
            br.Number = number;
            br.TransactionsCount = $"{transactionsCount}";
            br.Timestamp = timestamp;
            br.LiveCellChanges = $"{outputsCount - inputsCount}";
            // TODO: update this
            br.Reward = "4000000000";
            // TODO: update this
            br.MinerHash = "ckt1qyqrdsefa43s6m882pcj53m4gdnj4k440axqswmu83";

            return br;
        }

        private static List<UInt64> GetBlockNumbers(UInt64 tipBlockNumber, int count)
        {
            List<UInt64> nums = new();
            for (int i = 0; i < count; i++)
            {
                if (tipBlockNumber < (UInt64)i)
                {
                    break;
                }
                nums.Add(tipBlockNumber - (UInt64)i);
            }
            return nums;
        }
    }

    public class BlocksResult
    {
        [JsonPropertyName("miner_hash")]
        public string MinerHash { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }

        [JsonPropertyName("reward")]
        public string Reward { get; set; }

        [JsonPropertyName("transactions_count")]
        public string TransactionsCount { get; set; }

        [JsonPropertyName("live_cell_changes")]
        public string LiveCellChanges { get; set; }
    }
}
