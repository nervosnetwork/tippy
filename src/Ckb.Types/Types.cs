using System;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Ckb.Types
{
    public class Block
    {
        [JsonProperty(PropertyName = "header")]
        public Header Header { get; set; } = default;

        [JsonProperty(PropertyName = "proposals")]
        public string[] Proposals { get; set; } = Array.Empty<string>();

        [JsonProperty(PropertyName = "transactions")]
        public Transaction[] Transactions { get; set; } = Array.Empty<Transaction>();

        [JsonProperty(PropertyName = "uncles")]
        public Uncle[] Uncles { get; set; } = Array.Empty<Uncle>();
    }

    public class Header
    {
        [JsonProperty(PropertyName = "compact_target")]
        public string CompactTarget { get; set; }

        [JsonProperty(PropertyName = "dao")]
        public string Dao { get; set; }

        [JsonProperty(PropertyName = "epoch")]
        public string Epoch { get; set; }

        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; }

        [JsonProperty(PropertyName = "nonce")]
        public string Nonce { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "parent_hash")]
        public string ParentHash { get; set; }

        [JsonProperty(PropertyName = "proposals_hash")]
        public string ProposalsHash { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public string Timestamp { get; set; }

        [JsonProperty(PropertyName = "transactions_root")]
        public string TransactionsRoot { get; set; }

        [JsonProperty(PropertyName = "uncles_hash")]
        public string UnclesHash { get; set; }

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; }
    }

    public class Transaction
    {
        [JsonProperty(PropertyName = "cell_deps")]
        public CellDep[] CellDeps { get; set; } = Array.Empty<CellDep>();

        // can be null
        [JsonProperty(PropertyName = "hash")]
        public string Hash { get; set; } = null;

        [JsonProperty(PropertyName = "header_deps")]
        public string[] HeaderDeps { get; set; } = Array.Empty<string>();

        [JsonProperty(PropertyName = "inputs")]
        public Input[] Inputs { get; set; } = Array.Empty<Input>();

        [JsonProperty(PropertyName = "outputs")]
        public Output[] Outputs { get; set; } = Array.Empty<Output>();

        [JsonProperty(PropertyName = "outputs_data")]
        public string[] OutputsData { get; set; } = Array.Empty<string>();

        [JsonProperty(PropertyName = "version")]
        public string Version { get; set; } = default;

        [JsonProperty(PropertyName = "witnesses")]
        public string[] Witnesses { get; set; } = Array.Empty<string>();
    }

    public class TxStatus
    {
        // can be null
        [JsonProperty(PropertyName = "block_hash")]
        public string BlockHash { get; set; } = null;

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; }
    }

    public class TransactionWithStatus
    {
        [JsonProperty(PropertyName = "transaction")]
        public Transaction Transaction { get; set; }

        [JsonProperty(PropertyName = "tx_status")]
        public TxStatus TxStatus { get; set; }
    }


    public class CellDep
    {
        [JsonProperty(PropertyName = "dep_type")]
        public string DepType { get; set; }

        [JsonProperty(PropertyName = "out_point")]
        public OutPoint OutPoint { get; set; }
    }

    public class OutPoint
    {
        [JsonProperty(PropertyName = "index")]
        public string Index { get; set; }

        [JsonProperty(PropertyName = "tx_hash")]
        public string TxHash { get; set; }
    }

    public class Input
    {
        [JsonProperty(PropertyName = "previous_output")]
        public OutPoint PreviousOutput { get; set; }

        [JsonProperty(PropertyName = "since")]
        public string Since { get; set; }
    }

    public class Output
    {
        [JsonProperty(PropertyName = "capacity")]
        public string Capacity { get; set; }

        [JsonProperty(PropertyName = "lock")]
        public Script Lock { get; set; }

        // can be null
        [JsonProperty(PropertyName = "type")]
        public Script Type { get; set; } = null;
    }

    public class Script
    {
        [JsonProperty(PropertyName = "args")]
        public string Args { get; set; }

        [JsonProperty(PropertyName = "code_hash")]
        public string CodeHash { get; set; }

        [JsonProperty(PropertyName = "hash_type")]
        public string HashType { get; set; }

        public static bool operator ==(Script left, Script right)
        {
            if (ReferenceEquals(left, right))
            {
                return true;
            }
            if (left is null || right is null)
            {
                return false;
            }
            return left.CodeHash == right.CodeHash && left.HashType == right.HashType && left.Args == right.Args;
        }

        public static bool operator !=(Script left, Script right)
        {
            return !(left == right);
        }

        public override bool Equals(object obj)
        {
            Script script = obj as Script;
            if (script == null)
            {
                return false;
            }
            return CodeHash == script.CodeHash && HashType == script.HashType && Args == script.Args;
        }

        public override int GetHashCode()
        {
            return CodeHash.GetHashCode() + HashType.GetHashCode() + Args.GetHashCode();
        }
    }

    public class Uncle
    {
        [JsonProperty(PropertyName = "proposals")]
        public string[] Proposals { get; set; } = Array.Empty<string>();

        [JsonProperty(PropertyName = "header")]
        public Header Header { get; set; }
    }

    public class BlockEconomicState
    {
        [JsonProperty(PropertyName = "finalized_at")]
        public string FinalizedAt { get; set; }

        [JsonProperty(PropertyName = "issuance")]
        public BlockIssuance Issuance { get; set; }

        [JsonProperty(PropertyName = "miner_reward")]
        public MinerReward MinerReward { get; set; }

        [JsonProperty(PropertyName = "txs_fee")]
        public string TxsFee { get; set; }
    }

    public class BlockIssuance
    {
        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }

        [JsonProperty(PropertyName = "secondary")]
        public string Secondary { get; set; }
    }

    public class MinerReward
    {
        [JsonProperty(PropertyName = "committed")]
        public string Committed { get; set; }

        [JsonProperty(PropertyName = "primary")]
        public string Primary { get; set; }

        [JsonProperty(PropertyName = "proposal")]
        public string Proposal { get; set; }

        [JsonProperty(PropertyName = "secondary")]
        public string Secondary { get; set; }
    }

    public class EpochView
    {
        [JsonProperty(PropertyName = "compact_target")]
        public string CompactTarget { get; set; }

        [JsonProperty(PropertyName = "length")]
        public string Length { get; set; }

        [JsonProperty(PropertyName = "number")]
        public string Number { get; set; }

        [JsonProperty(PropertyName = "start_number")]
        public string StartNumber { get; set; }
    }

    public class TxPoolInfo
    {
        [JsonProperty(PropertyName = "tip_hash")]
        public string TipHash { get; set; }

        [JsonProperty(PropertyName = "tip_number")]
        public string TipNumber { get; set; }

        [JsonProperty(PropertyName = "pending")]
        public string Pending { get; set; }

        [JsonProperty(PropertyName = "proposed")]
        public string Proposed { get; set; }

        [JsonProperty(PropertyName = "orphan")]
        public string Orphan { get; set; }

        [JsonProperty(PropertyName = "total_tx_size")]
        public string TotalTxSize { get; set; }

        [JsonProperty(PropertyName = "total_tx_cycles")]
        public string TotalTxCycles { get; set; }

        [JsonProperty(PropertyName = "min_fee_rate")]
        public string MinFeeRate { get; set; }

        [JsonProperty(PropertyName = "last_txs_updated_at")]
        public string LastTxsUpdatedAt { get; set; }
    }

    public class TxVerbosity
    {
        [JsonProperty(PropertyName = "cycles")]
        public string Cycles { get; set; }

        [JsonProperty(PropertyName = "size")]
        public string Size { get; set; }

        [JsonProperty(PropertyName = "fee")]
        public string Fee { get; set; }

        [JsonProperty(PropertyName = "ancestors_size")]
        public string AncestorsSize { get; set; }

        [JsonProperty(PropertyName = "ancestors_cycles")]
        public string AncestorsCycles { get; set; }

        [JsonProperty(PropertyName = "ancestors_count")]
        public string AncestorsCount { get; set; }
    }

    public class RawTxPool
    {
        [JsonProperty(PropertyName = "pending")]
        public Dictionary<string, TxVerbosity> Pending { get; set; }

        [JsonProperty(PropertyName = "proposed")]
        public Dictionary<string, TxVerbosity> Proposed { get; set; }
    }
}
