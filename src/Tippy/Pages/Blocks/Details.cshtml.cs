using System;
using System.Linq;
using System.Threading.Tasks;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.ApiData;
using Tippy.Core.Models;
using Tippy.Util;

namespace Tippy.Pages.Blocks
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public BlockDetailResult BlockDetail { get; set; } = default!;

        public IActionResult OnGet(int? id)
        {
            // id could be either a number or a hash
            if (id == null)
            {
                return NotFound();
            }

            if (ActiveProject == null)
            {
                return NotFound();
            }

            Client client = new($"http://localhost:{ActiveProject.NodeRpcPort}");

            Block? block = client.GetBlockByNumber((ulong)id);
            if (block == null)
            {
                return NotFound();
            }

            BlockDetail = new()
            {
                BlockHash = block.Header.Hash,
                TransactionsRoot = block.Header.TransactionsRoot,
                Number = $"{id}",
                Version = $"{Hex.HexToUInt32(block.Header.Version)}",
                ProposalsCount = $"{block.Proposals.Length}",
                UnclesCount = $"{block.Uncles.Length}",
                Timestamp = $"{Hex.HexToUInt64(block.Header.Timestamp)}",
                TransactionsCount = $"{block.Transactions.Length}",
                Nonce = $"{Hex.HexToBigInteger(block.Header.Nonce)}",
                Difficulty = Difficulty.CompactToDifficulty(Hex.HexToUInt32(block.Header.CompactTarget)).ToString()
            };

            // Miner Address
            string prefix = ActiveProject.Chain == Project.ChainType.Mainnet ? "ckb" : "ckt";
            string cellbaseWitness = block.Transactions[0].Witnesses[0];
            Script script = CellbaseWitness.Parse(cellbaseWitness);
            string minerAddress = Ckb.Address.Address.GenerateAddress(script, prefix);
            BlockDetail.MinerHash = minerAddress;

            // Epoch info
            var epochInfo = EpochInfo.Parse(Hex.HexToUInt64(block.Header.Epoch));
            BlockDetail.StartNumber = "0";
            BlockDetail.Length = epochInfo.Length.ToString();
            BlockDetail.Epoch = epochInfo.Number.ToString();
            BlockDetail.BlockIndexInEpoch = epochInfo.Index.ToString();

            EpochView? epochView = client.GetEpochByNumber(epochInfo.Number);
            if (epochView != null)
            {
                BlockDetail.StartNumber = Hex.HexToUInt64(epochView.StartNumber).ToString();
            }

            // reward
            string blockHash = block.Header.Hash;
            BlockDetail.RewardStatus = "pending";
            BlockDetail.MinerReward = "";
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
                BlockDetail.MinerReward = minerReward.ToString();
                BlockDetail.RewardStatus = "issued";
            }

            return Page();
        }
    }
}
