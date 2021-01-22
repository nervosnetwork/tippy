using System;
using Newtonsoft.Json;

namespace Ckb.Types.IndexrTypes
{
    public class CellCapacity
    {
        [JsonProperty(PropertyName = "block_hash")]
        public string BlockHash { get; set; }

        [JsonProperty(PropertyName = "block_number")]
        public string BlockNumber { get; set; }

        [JsonProperty(PropertyName = "capacity")]
        public string Capacity { get; set; }
    }


    public class Cell
    {
        [JsonProperty(PropertyName = "block_number")]
        public string BlockNumber { get; set; }

        [JsonProperty(PropertyName = "out_point")]
        public OutPoint OutPoint { get; set; }

        [JsonProperty(PropertyName = "output")]
        public Output Output { get; set; }

        [JsonProperty(PropertyName = "output_data")]
        public string OutputData { get; set; }

        [JsonProperty(PropertyName = "tx_index")]
        public string TxIndex { get; set; }
    }


    public class Transaction
    {
        [JsonProperty(PropertyName = "block_number")]
        public string BlockNumber { get; set; }

        [JsonProperty(PropertyName = "io_index")]
        public string IoIndex { get; set; }

        [JsonProperty(PropertyName = "io_type")]
        public string IoType { get; set; }

        [JsonProperty(PropertyName = "tx_hash")]
        public string TxHash { get; set; }

        [JsonProperty(PropertyName = "tx_index")]
        public string TxIndex { get; set; }
    }

    public class Result<T>
    {
        [JsonProperty(PropertyName = "last_cursor")]
        public string LastCursor { get; set; }

        [JsonProperty(PropertyName = "objects")]
        public T[] Objects { get; set; }
    }

    public class SearchKey
    {
        [JsonProperty(PropertyName = "script")]
        public Script Script { get; set; }

        [JsonProperty(PropertyName = "script_type")]
        public string ScriptType { get; set; }

        public SearchKey(Script script, string scriptType)
        {
            Script = script;
            ScriptType = scriptType;
        }
    }
}
