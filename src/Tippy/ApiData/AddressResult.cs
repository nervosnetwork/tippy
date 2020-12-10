using System.Text.Json.Serialization;

namespace Tippy.ApiData
{
    public class AddressResult
    {
        [JsonPropertyName("address_hash")]
        public string AddressHash { get; set; } = default!;

        [JsonPropertyName("lock_script")]
        public Ckb.Types.Script LockScript { get; set; } = default!;

        [JsonPropertyName("balance")]
        public string Balance { get; set; } = default!;

        [JsonPropertyName("transactions_count")]
        public string TransactionsCount { get; set; } = "0";

        [JsonPropertyName("live_cells_count")]
        public string LiveCellsCount { get; set; } = "0";

        [JsonPropertyName("mined_blocks_count")]
        public string MinedBlocksCount { get; set; } = "0";

        // sUDT
        //[JsonPropertyName("udt_accounts")]
        //public UdtAccountResult[] UdtAccounts { get; set; } = default!;

        // Dao, not support now.
        //[JsonPropertyName("dao_deposit")]
        //public string DaoDeposit { get; set; } = default!;

        //[JsonPropertyName("interest")]
        //public string Interest { get; set; } = default!;

        //[JsonPropertyName("average_deposit_time")]
        //public string AverageDepositTime { get; set; } = default!;

        //[JsonPropertyName("dao_compensation")]
        //public string DaoCompensation { get; set; } = default!;
    }

    public class UdtAccountResult
    {
        [JsonPropertyName("symbol")]
        public string Symbol { get; set; } = default!;

        [JsonPropertyName("decimal")]
        public string Decimal { get; set; } = default!;

        [JsonPropertyName("amount")]
        public string Amount { get; set; } = default!;

        [JsonPropertyName("type_hash")]
        public string TypeHash { get; set; } = default!;

        [JsonPropertyName("udt_icon_file")]
        public string UdtIconFile { get; set; } = default!;
    }
}
