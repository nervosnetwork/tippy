using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;
using System.Collections.Generic;
using Ckb.Rpc;
using Ckb.Types;
using Tippy.ApiData;
using Tippy.Util;

namespace Tippy.Controllers.API
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class BlocksController : ApplicationController
    {
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
                ArrayResult<BlockResult> r = GetBlocks(client, tipBlockNumber, 10);
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
            ArrayResult<BlockResult> result = GetBlocks(client, startBlockNumber, (int)pageSize, meta);

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
            blockDetail.TransactionsRoot = block.Header.TransactionsRoot;
            blockDetail.Number = $"{id}";
            blockDetail.Version = $"{Hex.HexToUInt32(block.Header.Version)}";
            blockDetail.ProposalsCount = $"{block.Proposals.Length}";
            blockDetail.UnclesCount = $"{block.Uncles.Length}";
            blockDetail.Timestamp = $"{Hex.HexToUInt64(block.Header.Timestamp)}";
            blockDetail.TransactionsCount = $"{block.Transactions.Length}";
            blockDetail.Nonce = $"{Hex.HexToBigInteger(block.Header.Nonce)}";
            blockDetail.Difficulty = Hex.HexToUInt32(block.Header.CompactTarget).ToString();

            // Miner Address
            string prefix = IsMainnet() ? "ckb" : "ckt";
            string cellbaseWitness = block.Transactions[0].Witnesses[0];
            Script script = CellbaseWitness.Parse(cellbaseWitness);
            string minerAddress = Ckb.Address.Address.GenerateAddress(script, prefix);
            blockDetail.MinerHash = minerAddress;

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

        private ArrayResult<BlockResult> GetBlocks(Client client, UInt64 startBlockNumber, int count, Meta? meta = null)
        {
            List<UInt64> blockNumbers = GetBlockNumbers(startBlockNumber, count);

            BlockResult[] brs = blockNumbers.Select(num => GetBlock(client, num)).OfType<BlockResult>().ToArray();
            ArrayResult<BlockResult> result = new("block_list", brs, meta);

            return result;
        }

        private BlockResult? GetBlock(Client client, UInt64 num)
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

            BlockResult br = new()
            {
                Number = number,
                TransactionsCount = $"{transactionsCount}",
                Timestamp = timestamp,
                LiveCellChanges = $"{outputsCount - inputsCount}",
                Reward = "",
                MinerHash = ""
            };

            // Reward
            string blockHash = header.Hash;
            BlockEconomicState? economicState = client.GetBlockEconomicState(blockHash);
            if (economicState != null)
            {
                MinerReward reward = economicState.MinerReward;
                string[] rewards = new string[]
                {
                    reward.Primary,
                    reward.Secondary
                };
                br.Reward = rewards.Select(r => Hex.HexToUInt64(r)).Aggregate((sum, cur) => sum + cur).ToString();
            }

            // Miner Address
            string prefix = IsMainnet() ? "ckb" : "ckt";
            string cellbaseWitness = block.Transactions[0].Witnesses[0];
            Script script = CellbaseWitness.Parse(cellbaseWitness);
            string minerAddress = Ckb.Address.Address.GenerateAddress(script, prefix);
            br.MinerHash = minerAddress;

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
}
