using System.Text.Json.Serialization;

namespace Ckb.Types.IndexrTypes
{
    public class CellCapacity
    {
        [JsonPropertyName("block_hash")]
        public string BlockHash { get; set; } = default!;

        [JsonPropertyName("block_number")]
        public string BlockNumber { get; set; } = default!;

        [JsonPropertyName("capacity")]
        public string Capacity { get; set; } = default!;
    }


    public class Cell
    {
        [JsonPropertyName("block_number")]
        public string BlockNumber { get; set; } = default!;

        [JsonPropertyName("out_point")]
        public OutPoint OutPoint { get; set; } = default!;

        [JsonPropertyName("output")]
        public Output Output { get; set; } = default!;

        [JsonPropertyName("output_data")]
        public string OutputData { get; set; } = default!;

        [JsonPropertyName("tx_index")]
        public string TxIndex { get; set; } = default!;
    }


    public class Transaction
    {
        [JsonPropertyName("block_number")]
        public string BlockNumber { get; set; } = default!;

        [JsonPropertyName("io_index")]
        public string IoIndex { get; set; } = default!;

        [JsonPropertyName("io_type")]
        public string IoType { get; set; } = default!;

        [JsonPropertyName("tx_hash")]
        public string TxHash { get; set; } = default!;

        [JsonPropertyName("tx_index")]
        public string TxIndex { get; set; } = default!;
    }

    public class Result<T>
    {
        [JsonPropertyName("last_cursor")]
        public string LastCursor { get; set; } = default!;

        [JsonPropertyName("objects")]
        public T[] Objects { get; set; } = default!;
    }

    public class SearchKey
    {
        [JsonPropertyName("script")]
        public Script Script { get; set; } = default!;

        [JsonPropertyName("script_type")]
        public string ScriptType { get; set; } = default!;

        public SearchKey(Script script, string scriptType)
        {
            Script = script;
            ScriptType = scriptType;
        }
    }
}
