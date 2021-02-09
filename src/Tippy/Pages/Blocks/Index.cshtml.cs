using System;
using System.Collections.Generic;
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

        public List<BlockResult> Result = default!;
        public int PageSize = 15;
        public int Total { get; set; } = 0;

        public void OnGet([FromQuery(Name = "e")] int? end)
        {
            if (ActiveProject == null || !ProcessManager.IsRunning(ActiveProject))
            {
                return;
            }

            var client = Rpc();

            int tipBlockNumber = (int)client.GetTipBlockNumber();
            Total = (int)tipBlockNumber + 1;
            int endBlock = end ?? tipBlockNumber;
            if (endBlock > tipBlockNumber)
            {
                endBlock = tipBlockNumber;
            }
            int startBlock = Math.Max(0, endBlock - PageSize + 1);
            Result = GetBlocks(client, startBlock, endBlock);
        }

        private List<BlockResult> GetBlocks(Client client, int startBlockNumber, int endBlockNumber)
        {
            return Enumerable.Range(startBlockNumber, endBlockNumber - startBlockNumber + 1)
                .Select(num => GetBlock(client, num))
                .OfType<BlockResult>()
                .Reverse()
                .ToList();
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

            ulong number = Hex.HexToUInt64(header.Number);
            int transactionsCount = transactions.Length;
            string timestamp = $"{ Hex.HexToUInt64(header.Timestamp) }";

            // Reward
            string CalcluateReward()
            {
                string blockHash = header.Hash;
                BlockEconomicState? economicState = client.GetBlockEconomicState(blockHash);
                if (economicState != null)
                {
                    MinerReward reward = economicState.MinerReward;
                    return Hex.HexToUInt64(reward.Primary).ToString();
                }
                else
                {
                    EpochInfo epochInfo = EpochInfo.Parse(Hex.HexToUInt64(block.Header.Epoch));
                    try
                    {
                        ulong primaryReward = PrimaryReward(client, number, epochInfo.Number);
                        return primaryReward.ToString();
                    }
                    catch
                    {
                        return "";
                    }
                }
            }

            // Miner Address
            string prefix = IsMainnet() ? "ckb" : "ckt";
            string cellbaseWitness = block.Transactions[0].Witnesses[0];
            Script script = CellbaseWitness.Parse(cellbaseWitness);
            string minerAddress = Ckb.Address.Address.GenerateAddress(script, prefix);

            BlockResult result = new()
            {
                Number = number.ToString(),
                BlockHash = header.Hash,
                TransactionsCount = $"{transactionsCount}",
                Timestamp = timestamp,
                LiveCellChanges = $"{outputsCount - inputsCount}",
                Reward = CalcluateReward(),
                MinerHash = minerAddress
            };

            return result;
        }

        private static ulong PrimaryReward(Client client, ulong blockNumber, ulong epochNumber)
        {
            if (blockNumber < 12)
            {
                return 0;
            }

            EpochView? epochInfo = client.GetEpochByNumber(epochNumber);
            if (epochInfo == null)
            {
                throw new Exception($"No Epoch found by number: {epochNumber}");
            }

            ulong startNumber = Hex.HexToUInt64(epochInfo.StartNumber);
            ulong length = Hex.HexToUInt64(epochInfo.Length);
            ulong epochReward = 1_917_808_21917808;
            ulong primaryReward = epochReward / length;
            ulong remainderReward = epochReward % length;

            if (blockNumber >= startNumber && blockNumber < startNumber + remainderReward)
            {
                return primaryReward + 1;
            }
            return primaryReward;
        }
    }
}
