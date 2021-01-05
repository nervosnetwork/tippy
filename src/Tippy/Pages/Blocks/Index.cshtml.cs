using System;
using System.Linq;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Util;

namespace Tippy.Pages.Blocks
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public ArrayResult<BlockResult> Result = default!;

        public void OnGet([FromQuery(Name = "e")] int? end)
        {
            if (ActiveProject == null || !ProcessManager.IsRunning(ActiveProject))
            {
                return;
            }

            var client = Rpc();

            int pageSize = 20;
            int tipBlockNumber = (int)client.GetTipBlockNumber();
            int endBlock = end ?? tipBlockNumber;
            if (endBlock > tipBlockNumber)
            {
                endBlock = tipBlockNumber;
            }
            int startBlock = Math.Max(0, endBlock - pageSize + 1);

            Meta meta = new()
            {
                Total = (UInt64)tipBlockNumber + 1,
                PageSize = pageSize
            };
            Result = GetBlocks(client, startBlock, endBlock, meta);
        }

        private ArrayResult<BlockResult> GetBlocks(Client client, int startBlockNumber, int endBlockNumber, Meta? meta = null)
        {
            BlockResult[] blockResults = Enumerable.Range(startBlockNumber, endBlockNumber - startBlockNumber + 1)
                .Select(num => GetBlock(client, num))
                .OfType<BlockResult>()
                .Reverse()
                .ToArray();
            return new ArrayResult<BlockResult>("block_list", blockResults, meta);
        }

        private BlockResult? GetBlock(Client client, int num)
        {
            Block? block = client.GetBlockByNumber((UInt64)num);
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

            // Reward
            string CalcluateReward()
            {
                string blockHash = header.Hash;
                BlockEconomicState? economicState = client.GetBlockEconomicState(blockHash);
                if (economicState == null)
                {
                    return "";
                }

                MinerReward reward = economicState.MinerReward;
                string[] rewards = new string[]
                {
                    reward.Primary,
                    reward.Secondary
                };
                return rewards.Select(r => Hex.HexToUInt64(r)).Aggregate((sum, cur) => sum + cur).ToString();
            }

            // Miner Address
            string prefix = IsMainnet() ? "ckb" : "ckt";
            string cellbaseWitness = block.Transactions[0].Witnesses[0];
            Script script = CellbaseWitness.Parse(cellbaseWitness);
            string minerAddress = Ckb.Address.Address.GenerateAddress(script, prefix);

            BlockResult result = new()
            {
                Number = number,
                BlockHash = header.Hash,
                TransactionsCount = $"{transactionsCount}",
                Timestamp = timestamp,
                LiveCellChanges = $"{outputsCount - inputsCount}",
                Reward = CalcluateReward(),
                MinerHash = minerAddress
            };

            return result;
        }
    }
}
