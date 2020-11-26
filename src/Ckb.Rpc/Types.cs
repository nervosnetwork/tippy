using System.Text.Json.Serialization;

namespace Ckb.Rpc.Types
{

    public class Block
    {
        public Header header { get; set; }
        public string[] proposals { get; set; }
        public Transaction[] transactions { get; set; }
        public Uncle[] uncles { get; set; }
    }

    public class Header
    {
        public string compact_target { get; set; }
        public string dao { get; set; }
        public string epoch { get; set; }
        public string hash { get; set; }
        public string nonce { get; set; }
        public string number { get; set; }
        public string parent_hash { get; set; }
        public string proposals_hash { get; set; }
        public string timestamp { get; set; }
        public string transactions_root { get; set; }
        public string uncles_hash { get; set; }
        public string version { get; set; }
    }

    public class Transaction
    {
        public CellDeps[] cell_deps { get; set; }
        public string? hash { get; set; }
        public string[] header_deps { get; set; }
        public Input[] inputs { get; set; }
        public Output[] outputs { get; set; }
        public string[] outputs_data { get; set; }
        public string version { get; set; }
        public string[] witnesses { get; set; }
    }

    public class CellDeps
    {
        public string dep_type { get; set; }
        public OutPoint out_point { get; set; }
    }

    public class OutPoint
    {
        public string index { get; set; }
        public string tx_hash { get; set; }
    }

    public class Input
    {
        public OutPoint previous_output { get; set; }
        public string since { get; set; }
    }

    public class Output
    {
        public string capacity { get; set; }

        [JsonPropertyName("lock")]
        public Script Lock { get; set; }
        public Script? type { get; set; }
    }

    public class Script
    {
        public string args { get; set; }
        public string code_hash { get; set; }
        public string hash_type { get; set; }
    }

    public class Uncle
    {
        public string[] proposals { get; set; }
        public Header header { get; set; }
    }

}
