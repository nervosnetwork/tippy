using System;
using System.Collections.Generic;
using Hex = Ckb.Types.Convert;

namespace Ckb.Rpc
{
    public class Client : BaseClient
    {
        public Client(string url) : base(url) { }

        public UInt64 GetTipBlockNumber()
        {
            var result = Call<string>("get_tip_block_number") ?? "0x0";
            return Hex.HexToUInt64(result);
        }

        public Dictionary<string, object> GetBlockchainInfo()
        {
            var fallback = new Dictionary<string, object> { };
            return Call<Dictionary<string, object>>("get_blockchain_info") ?? fallback;
        }

        public Types.Block? GetBlockByNumber(UInt64 num)
        {
            string[] methodParams = { Hex.UInt64ToHex(num) };
            return Call<Types.Block>("get_block_by_number", methodParams);
        }

        public Types.Block? GetBlock(string hash)
        {
            string[] methodParams = { hash };
            return Call<Types.Block>("get_block", methodParams);
        }

        public Types.BlockEconomicState? GetBlockEconomicState(string blockHash)
        {
            string[] methodParams = { blockHash };
            return Call<Types.BlockEconomicState>("get_block_economic_state", methodParams);
        }

        public Types.EpochView? GetEpochByNumber(UInt64 epochNumber)
        {
            string[] methodParams = { Hex.UInt64ToHex(epochNumber) };
            return Call<Types.EpochView>("get_epoch_by_number", methodParams);
        }

        public Types.EpochView? GetCurrentEpoch() => Call<Types.EpochView>("get_current_epoch");

        public Types.TransactionWithStatus? GetTransaction(string hash)
        {
            string[] methodParams = { hash };
            return Call<Types.TransactionWithStatus>("get_transaction", methodParams);
        }

        public Types.Header? GetHeaderByNumber(UInt64 number)
        {
            string[] methodParams = { Hex.UInt64ToHex(number) };
            return Call<Types.Header>("get_header_by_number", methodParams);
        }

        public Types.Header? GetHeader(string blockHash)
        {
            string[] methodParams = { blockHash };
            return Call<Types.Header>("get_header", methodParams);
        }

        public String? GenerateBlock(/* block_assembler_script: Option<Script>, block_assembler_message: Option<JsonBytes>*/)
        {
            return Call<String>("generate_block");
        }

        public void Truncate(string targetTipHash)
        {
            Call<String>("truncate", new string[] { targetTipHash });
        }
    }
}
