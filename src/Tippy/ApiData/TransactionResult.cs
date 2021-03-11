using System;
using System.Text.Json.Serialization;
using Tippy.Helpers;

namespace Tippy.ApiData
{
    public class TransactionResult
    {
        [JsonPropertyName("transaction_hash")]
        public string TransactionHash { get; set; } = default!;

        [JsonPropertyName("block_number")]
        public string BlockNumber { get; set; } = default!;

        [JsonPropertyName("block_timestamp")]
        public string BlockTimestamp { get; set; } = default!;

        [JsonPropertyName("capacity_involved")]
        public string CapacityInvolved { get; set; } = default!;

        [JsonPropertyName("live_cell_changes")]
        public string LiveCellChanges { get; set; } = default!;

        public DateTime Date() => DateHelper.TimestampToDate(BlockTimestamp);
    }

    public class TransactionListResult
    {
        [JsonPropertyName("is_cellbase")]
        public bool IsCellbase { get; set; } = default!;

        [JsonPropertyName("transaction_hash")]
        public string TransactionHash { get; set; } = default!;

        [JsonPropertyName("block_number")]
        public string BlockNumber { get; set; } = default!;

        [JsonPropertyName("block_timestamp")]
        public string BlockTimestamp { get; set; } = default!;

        [JsonPropertyName("display_inputs")]
        public DisplayInput[] DisplayInputs { get; set; } = default!;

        [JsonPropertyName("display_outputs")]
        public DisplayOutput[] DisplayOutputs { get; set; } = default!;

        [JsonPropertyName("income")]
        public string? Income { get; set; } = null;

        public DateTime Date() => DateHelper.TimestampToDate(BlockTimestamp);
    }

    public class SudtInfo
    {
        [JsonPropertyName("amount")]
        public string Amount { get; set; } = default!;

        [JsonPropertyName("sudt_script_args")]
        public string SudtScriptArgs { get; set; } = default!;

        public string Name { get; set; } = "";

        public int Id { get; set; } = 0;
    }


    public class TransactionDetailResult : TransactionListResult
    {
        [JsonPropertyName("witnesses")]
        public string[] Witnesses { get; set; } = default!;

        [JsonPropertyName("cell_deps")]
        public CellDep[] CellDeps { get; set; } = default!;

        [JsonPropertyName("header_deps")]
        public string[] HeaderDeps { get; set; } = default!;

        [JsonPropertyName("tx_status")]
        public string TxStatus { get; set; } = default!;

        [JsonPropertyName("transaction_fee")]
        public string TransactionFee { get; set; } = default!;

        [JsonPropertyName("version")]
        public string Version { get; set; } = default!;
    }

    public class CellDep
    {
        [JsonPropertyName("dep_type")]
        public string DepType { get; set; } = default!;

        [JsonPropertyName("out_point")]
        public OutPoint OutPoint { get; set; } = default!;
    }

    public class OutPoint
    {
        [JsonPropertyName("index")]
        public uint Index { get; set; } = default!;

        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; } = default!;
    }

    public class DisplayInput
    {
        [JsonPropertyName("from_cellbase")]
        public bool FromCellbase { get; set; } = default!;

        [JsonPropertyName("capacity")]
        public string Capacity { get; set; } = default!;

        [JsonPropertyName("address_hash")]
        public string AddressHash { get; set; } = default!;

        [JsonPropertyName("generated_tx_hash")]
        public string GeneratedTxHash { get; set; } = default!;

        // For not cellbase
        [JsonPropertyName("cell_index")]
        public string? CellIndex { get; set; } = default!;

        // For not cellbase
        [JsonPropertyName("cell_type")]
        public string? CellType { get; set; } = default!;

        // For cellbase
        [JsonPropertyName("target_block_number")]
        public string? TargetBlockNumber { get; set; } = default!;

        // $"{txHash}:{index(int)}"
        [JsonPropertyName("id")]
        public string? Id { get; set; } = default!;

        [JsonPropertyName("sudt_info")]
        public SudtInfo? SudtInfo { get; set; } = null;

        [JsonPropertyName("occupied_capacity")]
        public string? OccupiedCapacity { get; set; } = null;
    }

    public class DisplayOutput
    {
        [JsonPropertyName("capacity")]
        public string Capacity { get; set; } = default!;

        [JsonPropertyName("address_hash")]
        public string AddressHash { get; set; } = default!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = default!;

        [JsonPropertyName("consumed_tx_hash")]
        public string ConsumedTxHash { get; set; } = default!;

        // For not cellbase
        [JsonPropertyName("cell_type")]
        public string? CellType { get; set; } = default!;

        // For cellbase
        [JsonPropertyName("target_block_number")]
        public string? TargetBlockNumber { get; set; } = default;

        [JsonPropertyName("base_reward")]
        public string? PrimaryReward { get; set; } = default!;

        [JsonPropertyName("commit_reward")]
        public string? CommitReward { get; set; } = default!;

        [JsonPropertyName("proposal_reward")]
        public string? ProposalReward { get; set; } = default!;

        [JsonPropertyName("secondary_reward")]
        public string? SecondaryReward { get; set; } = default!;

        // $"{txHash}:{index(int)}"
        [JsonPropertyName("id")]
        public string? Id { get; set; } = default!;

        [JsonPropertyName("sudt_info")]
        public SudtInfo? SudtInfo = null;

        [JsonPropertyName("occupied_capacity")]
        public string? OccupiedCapacity { get; set; } = null;
    }
}
