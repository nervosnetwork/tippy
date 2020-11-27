using System.Text.Json.Serialization;

namespace Ckb.Rpc.Types
{
#nullable disable
    public class Block
    {
        public Header Header { get; set; }
        public string[] Proposals { get; set; }
        public Transaction[] Transactions { get; set; }
        public Uncle[] Uncles { get; set; }
    }

    public class Header
    {
        public string CompactTarget { get; set; }
        public string Dao { get; set; }
        public string Epoch { get; set; }
        public string Hash { get; set; }
        public string Nonce { get; set; }
        public string Number { get; set; }
        public string ParentHash { get; set; }
        public string ProposalsHash { get; set; }
        public string Timestamp { get; set; }
        public string TransactionsRoot { get; set; }
        public string UnclesHash { get; set; }
        public string Version { get; set; }
    }

    public class Transaction
    {
        public CellDeps[] CellDeps { get; set; }
#nullable enable
        public string? Hash { get; set; }
#nullable disable
        public string[] HeaderDeps { get; set; }
        public Input[] Inputs { get; set; }
        public Output[] Outputs { get; set; }
        public string[] OutputsData { get; set; }
        public string Version { get; set; }
        public string[] Witnesses { get; set; }
    }

    public class CellDeps
    {
        public string DepType { get; set; }
        public OutPoint OutPoint { get; set; }
    }

    public class OutPoint
    {
        public string Index { get; set; }
        public string TxHash { get; set; }
    }

    public class Input
    {
        public OutPoint PreviousOutput { get; set; }
        public string Since { get; set; }
    }

    public class Output
    {
        public string Capacity { get; set; }

        [JsonPropertyName("lock")]
        public Script Lock { get; set; }
#nullable enable
        public Script? Type { get; set; }
#nullable disable
    }

    public class Script
    {
        public string Args { get; set; }
        public string CodeHash { get; set; }
        public string HashType { get; set; }
    }

    public class Uncle
    {
        public string[] Proposals { get; set; }
        public Header Header { get; set; }
    }
}
