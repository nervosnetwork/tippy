using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
