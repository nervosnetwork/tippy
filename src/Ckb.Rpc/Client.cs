using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using Tippy.Util;

namespace Ckb.Rpc
{
    public class Client
    {
        public Client(string url)
        {
            Url = new Uri(url);
        }

        readonly Uri Url;

        public ResponseObject? Call(string method, params object[]? methodParams)
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
            return JsonSerializer.Deserialize<ResponseObject>(responseReader.ReadToEnd());
        }

        public UInt64 GetTipBlockNumber()
        {
            var result = Call("get_tip_block_number")?.Result?.ToString() ?? "0x0";
            return Hex.HexToUInt64(result);
        }

        public Dictionary<string, object> GetBlockchainInfo()
        {
            var fallback = new Dictionary<string, object> { };
            var result = Call("get_blockchain_info")?.Result?.ToString() ?? "{}";
            return JsonSerializer.Deserialize<Dictionary<string, object>>(result) ?? fallback;
        }

        public Types.Block? GetBlockByNumber(UInt64 num)
        {
            string[] methodParams = { Hex.UInt64ToHex(num) };
            var result = Call("get_block_by_number", methodParams)?.Result?.ToString();
            if (result == null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<Types.Block>(result);
        }

        public Types.BlockEconomicState? GetBlockEconomicState(string blockHash)
        {
            string[] methodParams = { blockHash };
            string? result = Call("get_block_economic_state", methodParams)?.Result?.ToString();
            if (result == null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<Types.BlockEconomicState>(result);
        }

        public Types.EpochView? GetEpochByNumber(UInt64 epochNumber)
        {
            string[] methodParams = { Hex.UInt64ToHex(epochNumber) };
            string? result = Call("get_epoch_by_number", methodParams)?.Result?.ToString();
            if (result == null)
            {
                return null;
            }
            return JsonSerializer.Deserialize<Types.EpochView>(result);
        }
    }

    public class RequestObject
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

    public class ResponseObject
    {
        [JsonPropertyName("jsonrpc")]
        public string Jsonrpc { get; set; } = "2.0";

        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("result")]
        public object? Result { get; set; }
    }

    public class SnakeCaseJsonNamingPolicy : JsonNamingPolicy
    {
        public override string ConvertName(string name) =>
            string.Concat(name.Select((x, i) => i > 0 && char.IsUpper(x) ? "_" + x.ToString() : x.ToString()))
                .ToLower();
    }
}
