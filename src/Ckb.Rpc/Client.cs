using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tippy.Util;
using IndexerTypes = Ckb.Types.IndexrTypes;

namespace Ckb.Rpc
{
    public class BaseClient
    {
        public BaseClient(string url)
        {
            Url = new Uri(url);
        }

        readonly Uri Url;

        public T? Call<T>(string method, params object[]? methodParams)
        {
            HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(Url);
            webRequest.ContentType = "application/json";
            webRequest.Method = "POST";
            webRequest.KeepAlive = true;

            var request = new RequestObject
            {
                Method = method,
                Params = methodParams
            };
            var serialized = JsonSerializer.Serialize(request);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            webRequest.ContentLength = bytes.Length;
            using Stream body = webRequest.GetRequestStream();
            body.Write(bytes, 0, bytes.Length);

            using WebResponse webResponse = webRequest.GetResponse();
            using Stream responseStream = webResponse.GetResponseStream();
            using StreamReader responseReader = new StreamReader(responseStream);
            var response = JsonSerializer.Deserialize<ResponseObject<T>>(responseReader.ReadToEnd());
            if (response != null)
            {
                return response.Result;
            }
            return default;
        }
    }

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
    }

    public class IndexerClient : BaseClient
    {
        public IndexerClient(string url) : base(url) { }

        public IndexerTypes.CellCapacity? GetCellsCapacity(IndexerTypes.SearchKey searchKey)
        {
            object[] methodParams = { searchKey };
            return Call<IndexerTypes.CellCapacity>("get_cells_capacity", methodParams);
        }

        public IndexerTypes.Result<IndexerTypes.Cell>? GetCells(IndexerTypes.SearchKey searchKey, string order = "asc", int limit = 100, string? afterCursor = null)
        {
            List<object> methodParams = new() { searchKey, order, Hex.Int32ToHex(limit) };
            if (afterCursor != null)
            {
                methodParams.Add(afterCursor);
            }
            return Call<IndexerTypes.Result<IndexerTypes.Cell>>("get_cells", methodParams.ToArray());
        }

        public IndexerTypes.Result<IndexerTypes.Transaction>? GetTransactions(IndexerTypes.SearchKey searchKey, string order = "asc", int limit = 100, string? afterCursor = null)
        {
            List<object> methodParams = new() { searchKey, order, Hex.Int32ToHex(limit) };
            if (afterCursor != null)
            {
                methodParams.Add(afterCursor);
            }
            return Call<IndexerTypes.Result<IndexerTypes.Transaction>>("get_transactions", methodParams.ToArray());
        }
    }

    class RequestObject
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; } = "2.0";

        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("method")]
        public string Method { get; set; } = "";

        [JsonPropertyName("params")]
        public object[]? Params { get; set; }
    }

    class ResponseObject<T>
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("result")]
        public T? Result { get; set; }
    }

    public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();
    }
}
