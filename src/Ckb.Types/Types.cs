using System.Text.Json.Serialization;

namespace Ckb.Types
{
#nullable disable
    public class Block
    {
        [JsonPropertyName("header")]
        public Header Header { get; set; }
        [JsonPropertyName("proposals")]
        public string[] Proposals { get; set; }
        [JsonPropertyName("transactions")]
        public Transaction[] Transactions { get; set; }
        [JsonPropertyName("uncles")]
        public Uncle[] Uncles { get; set; }
    }

    public class Header
    {
        [JsonPropertyName("compact_target")]
        public string CompactTarget { get; set; }
        [JsonPropertyName("dao")]
        public string Dao { get; set; }
        [JsonPropertyName("epoch")]
        public string Epoch { get; set; }
        [JsonPropertyName("hash")]
        public string Hash { get; set; }
        [JsonPropertyName("nonce")]
        public string Nonce { get; set; }
        [JsonPropertyName("number")]
        public string Number { get; set; }
        [JsonPropertyName("parent_hash")]
        public string ParentHash { get; set; }
        [JsonPropertyName("proposals_hash")]
        public string ProposalsHash { get; set; }
        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; }
        [JsonPropertyName("transactions_root")]
        public string TransactionsRoot { get; set; }
        [JsonPropertyName("uncles_hash")]
        public string UnclesHash { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
    }

    public class Transaction
    {
        [JsonPropertyName("cell_deps")]
        public CellDep[] CellDeps { get; set; }
#nullable enable
        [JsonPropertyName("hash")]
        public string? Hash { get; set; }
#nullable disable
        [JsonPropertyName("header_deps")]
        public string[] HeaderDeps { get; set; }
        [JsonPropertyName("inputs")]
        public Input[] Inputs { get; set; }
        [JsonPropertyName("outputs")]
        public Output[] Outputs { get; set; }
        [JsonPropertyName("outputs_data")]
        public string[] OutputsData { get; set; }
        [JsonPropertyName("version")]
        public string Version { get; set; }
        [JsonPropertyName("witnesses")]
        public string[] Witnesses { get; set; }
    }

    public class TxStatus
    {
        [JsonPropertyName("block_hash")]
        public string? BlockHash { get; set; }

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;
    }

    public class TransactionWithStatus
    {
        [JsonPropertyName("transaction")]
        public Transaction Transaction { get; set; } = default!;

        [JsonPropertyName("tx_status")]
        public TxStatus TxStatus { get; set; } = default!;
    }


    public class CellDep
    {
        [JsonPropertyName("dep_type")]
        public string DepType { get; set; }
        [JsonPropertyName("out_point")]
        public OutPoint OutPoint { get; set; }
    }

    public class OutPoint
    {
        [JsonPropertyName("index")]
        public string Index { get; set; }
        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; }
    }

    public class Input
    {
        [JsonPropertyName("previous_output")]
        public OutPoint PreviousOutput { get; set; }
        [JsonPropertyName("since")]
        public string Since { get; set; }
    }

    public class Output
    {
        [JsonPropertyName("capacity")]
        public string Capacity { get; set; }

        [JsonPropertyName("lock")]
        public Script Lock { get; set; }
#nullable enable
        [JsonPropertyName("type")]
        public Script? Type { get; set; }
#nullable disable
    }

    public class Script
    {
        [JsonPropertyName("args")]
        public string Args { get; set; }
        [JsonPropertyName("code_hash")]
        public string CodeHash { get; set; }
        [JsonPropertyName("hash_type")]
        public string HashType { get; set; }
    }

    public class Uncle
    {
        [JsonPropertyName("proposals")]
        public string[] Proposals { get; set; }
        [JsonPropertyName("header")]
        public Header Header { get; set; }
    }

    public class BlockEconomicState
    {
        [JsonPropertyName("finalized_at")]
        public string FinalizedAt { get; set; }

        [JsonPropertyName("issuance")]
        public BlockIssuance Issuance { get; set; }

        [JsonPropertyName("miner_reward")]
        public MinerReward MinerReward { get; set; }

        [JsonPropertyName("txs_fee")]
        public string TxsFee { get; set; }
    }

    public class BlockIssuance
    {
        [JsonPropertyName("primary")]
        public string Primary { get; set; }

        [JsonPropertyName("secondary")]
        public string Secondary { get; set; }
    }

    public class MinerReward
    {
        [JsonPropertyName("committed")]
        public string Committed { get; set; }

        [JsonPropertyName("primary")]
        public string Primary { get; set; }

        [JsonPropertyName("proposal")]
        public string Proposal { get; set; }

        [JsonPropertyName("secondary")]
        public string Secondary { get; set; }
    }

    public class EpochView
    {
        [JsonPropertyName("compact_target")]
        public string CompactTarget { get; set; }

        [JsonPropertyName("length")]
        public string Length { get; set; }

        [JsonPropertyName("number")]
        public string Number { get; set; }

        [JsonPropertyName("start_number")]
        public string StartNumber { get; set; }
    }
}
