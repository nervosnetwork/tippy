using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;

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
            var serialized = JsonConvert.SerializeObject(request);
            var bytes = Encoding.UTF8.GetBytes(serialized);
            webRequest.ContentLength = bytes.Length;
            using Stream body = webRequest.GetRequestStream();
            body.Write(bytes, 0, bytes.Length);

            using WebResponse webResponse = webRequest.GetResponse();
            using Stream responseStream = webResponse.GetResponseStream();
            using StreamReader responseReader = new StreamReader(responseStream);
            var response = JsonConvert.DeserializeObject<ResponseObject<T>>(responseReader.ReadToEnd());
            if (response != null)
            {
                return response.Result;
            }
            return default;
        }
    }

    class RequestObject
    {
        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc { get; } = "2.0";

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "1";

        [JsonProperty(PropertyName = "method")]
        public string Method { get; set; } = "";

        [JsonProperty(PropertyName = "params")]
        public object[]? Params { get; set; }
    }

    class ResponseObject<T>
    {
        [JsonProperty(PropertyName = "jsonrpc")]
        public string Jsonrpc { get; set; } = "2.0";

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = "1";

        [JsonProperty(PropertyName = "result")]
        public T? Result { get; set; }
    }
}
