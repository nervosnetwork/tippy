using System;
using System.Text.Json.Serialization;
using Tippy.Helpers;

namespace Tippy.ApiData
{
    public class BlockResult
    {
        [JsonPropertyName("miner_hash")]
        public string MinerHash { get; set; } = default!;

        [JsonPropertyName("number")]
        public string Number { get; set; } = default!;

        [JsonPropertyName("block_hash")]
        public string BlockHash { get; set; } = default!;

        [JsonPropertyName("timestamp")]
        public string Timestamp { get; set; } = default!;

        [JsonPropertyName("reward")]
        public string Reward { get; set; } = default!;

        [JsonPropertyName("transactions_count")]
        public string TransactionsCount { get; set; } = default!;

        [JsonPropertyName("live_cell_changes")]
        public string LiveCellChanges { get; set; } = default!;

        public DateTime Date() => DateHelper.TimestampToDate(Timestamp);
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

        public DateTime Date() => DateHelper.TimestampToDate(Timestamp);
    }
}
