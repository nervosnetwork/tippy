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
            Project? activeProject = HttpContext.Items["ActiveProject"] as Project;
            if (activeProject != null && ProcessManager.IsRunning(activeProject))
            {
                return new Client($"http://localhost:{activeProject.NodeRpcPort}");
            }
            return null;
        }

        [HttpGet]
        public ActionResult Index([FromQuery(Name = "page")] int? page, [FromQuery(Name = "page_size")] int? pageSize)
        {
            var client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            UInt64 tipBlockNumber = client.GetTipBlockNumber();
            if (page == null || pageSize == null)
            {
                ArrayResult<BlocksResult> r = GetBlocks(client, tipBlockNumber, 10);
                return Ok(r);
            }

            if (page < 1 || pageSize < 1)
            {
                return NoContent();
            }

            UInt64 skipCount = (UInt64)((page - 1) * pageSize);
            if (skipCount > tipBlockNumber)
            {
                return NoContent();
            }

            UInt64 startBlockNumber = tipBlockNumber - skipCount;
            Meta meta = new();
            meta.Total = tipBlockNumber + 1;
            meta.PageSize = (int)pageSize;
            ArrayResult<BlocksResult> result = GetBlocks(client, startBlockNumber, (int)pageSize, meta);

            return Ok(result);
        }

        // TODO: should use UInt64 here
        [HttpGet("{id:int}")]
        public ActionResult Details(UInt64 id)
        {
            var client = Rpc();
            if (client == null)
            {
                return NoContent();
            }

            Block? block = client.GetBlockByNumber(id);
            if (block == null)
            {
                return NoContent();
            }

            BlockDetailResult blockDetail = new();
            blockDetail.BlockHash = block.Header.Hash;
            // TODO: update this.
            blockDetail.MinerHash = "ckt";
            blockDetail.TransactionsRoot = block.Header.TransactionsRoot;
            blockDetail.Number = $"{id}";
            blockDetail.Version = $"{Hex.HexToUInt32(block.Header.Version)}";
            blockDetail.ProposalsCount = $"{block.Proposals.Length}";
            blockDetail.UnclesCount = $"{block.Uncles.Length}";
            blockDetail.Timestamp = $"{Hex.HexToUInt64(block.Header.Timestamp)}";
            blockDetail.TransactionsCount = $"{block.Transactions.Length}";
            blockDetail.Nonce = $"{Hex.HexToBigInteger(block.Header.Nonce)}";
            blockDetail.Difficulty = Hex.HexToUInt32(block.Header.CompactTarget).ToString();

            // Epoch info
            var epochInfo = EpochInfo.Parse(Hex.HexToUInt64(block.Header.Epoch));
            blockDetail.StartNumber = "0";
            blockDetail.Length = epochInfo.Length.ToString();
            blockDetail.Epoch = epochInfo.Number.ToString();
            blockDetail.BlockIndexInEpoch = epochInfo.Index.ToString();

            EpochView? epochView = client.GetEpochByNumber(epochInfo.Number);
            if (epochView != null)
            {
                blockDetail.StartNumber = Hex.HexToUInt64(epochView.StartNumber).ToString();
            }

            // reward
            string blockHash = block.Header.Hash;
            blockDetail.RewardStatus = "pending";
            blockDetail.MinerReward = "";
            BlockEconomicState? economicState = client.GetBlockEconomicState(blockHash);
            if (economicState != null)
            {
                MinerReward reward = economicState.MinerReward;
                string[] rewards = new string[]
                {
                    reward.Primary,
                    reward.Secondary,
                    reward.Proposal,
                    reward.Committed,
                };
                UInt64 minerReward = rewards.Select(r => Hex.HexToUInt64(r)).Aggregate((sum, cur) => sum + cur);
                blockDetail.MinerReward = minerReward.ToString();
                blockDetail.RewardStatus = "issued";
            }

            Result<BlockDetailResult> result = new("block", blockDetail);

            return Ok(result);
        }

        private static ArrayResult<BlocksResult> GetBlocks(Client client, UInt64 startBlockNumber, int count, Meta? meta = null)
        {
            List<UInt64> blockNumbers = GetBlockNumbers(startBlockNumber, count);

            BlocksResult[] brs = blockNumbers.Select(num => GetBlock(client, num)).OfType<BlocksResult>().ToArray();
            ArrayResult<BlocksResult> result = new("block_list", brs, meta);

            return result;
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

            BlocksResult br = new()
            {
                Number = number,
                TransactionsCount = $"{transactionsCount}",
                Timestamp = timestamp,
                LiveCellChanges = $"{outputsCount - inputsCount}",
                // TODO: update this
                Reward = "4000000000",
                // TODO: update this
                MinerHash = "ckt1qyqrdsefa43s6m882pcj53m4gdnj4k440axqswmu83"
            };

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
        public string MinerHash { get; set; } = default!;

        [JsonPropertyName("number")]
        public string Number { get; set; } = default!;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = default!;

        [JsonPropertyName("reward")]
        public string Reward { get; set; } = default!;

        [JsonPropertyName("transactions_count")]
        public string TransactionsCount { get; set; } = default!;

        [JsonPropertyName("live_cell_changes")]
        public string LiveCellChanges { get; set; } = default!;
    }



    public class BlockDetailResult
    {
        [JsonPropertyName("block_hash")]
        public string BlockHash { get; set; } = default!;

        [JsonPropertyName("miner_hash")]
        public string MinerHash { get; set; } = default!;

        [JsonPropertyName("transactions_root")]
        public string TransactionsRoot { get; set; } = default!;

        [JsonPropertyName("reward_status")]
        public string RewardStatus { get; set; } = default!;

        [JsonPropertyName("number")]
        public string Number { get; set; } = default!;

        [JsonPropertyName("start_number")]
        public string StartNumber { get; set; } = default!;

        [JsonPropertyName("length")]
        public string Length { get; set; } = default!;

        [JsonPropertyName("version")]
        public string Version { get; set; } = default!;

        [JsonPropertyName("proposals_count")]
        public string ProposalsCount { get; set; } = default!;

        [JsonPropertyName("uncles_count")]
        public string UnclesCount { get; set; } = default!;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = default!;

        [JsonPropertyName("transactions_count")]
        public string TransactionsCount { get; set; } = default!;

        [JsonPropertyName("epoch")]
        public string Epoch { get; set; } = default!;

        [JsonPropertyName("block_index_in_epoch")]
        public string BlockIndexInEpoch { get; set; } = default!;

        [JsonPropertyName("nonce")]
        public string Nonce { get; set; } = default!;

        [JsonPropertyName("difficulty")]
        public string Difficulty { get; set; } = default!;

        [JsonPropertyName("miner_reward")]
        public string MinerReward { get; set; } = default!;

    }
}
