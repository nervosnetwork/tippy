using System;
using System.Text.Json.Serialization;

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

        public DateTime Date()
        {
            DateTime date = new (1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            return date.AddSeconds(double.Parse(BlockTimestamp) / 1000).ToLocalTime();
        }
    }
}
