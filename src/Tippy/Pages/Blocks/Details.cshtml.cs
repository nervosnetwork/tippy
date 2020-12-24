using System;
using System.Linq;
using System.Threading.Tasks;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.ApiData;
using Tippy.Core.Models;
using Tippy.Filters;
using Tippy.Util;

namespace Tippy.Pages.Blocks
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class DetailsModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public DetailsModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public Project? ActiveProject { get; set; }
        public Result<BlockDetailResult> BlockResult { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            // id could be either a number or a hash
            if (id == null)
            {
                return NotFound();
            }

            ActiveProject = HttpContext.Items["ActiveProject"] as Project;
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
            uint compactTarget = Hex.HexToUInt32(block.Header.CompactTarget);
            blockDetail.Difficulty = Difficulty.CompactToDifficulty(compactTarget).ToString();

            // Miner Address
            string prefix = ActiveProject.Chain == Project.ChainType.Mainnet ? "ckb" : "ckt";
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

            BlockResult = new("block", blockDetail);
            if (BlockResult == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
