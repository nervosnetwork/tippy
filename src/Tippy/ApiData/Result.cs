using System;
using System.Text.Json.Serialization;

using Attributes = System.Collections.Generic.Dictionary<string, object>;

namespace Tippy.ApiData
{
    public class Result
    {
        public Result()
        {
        }

        public Result(Data data)
        {
            Data = data;
        }

        [JsonPropertyName("data")]
        public Data Data { get; set; }
    }

    public class Data
    { 
        public Data()
        {
        }

        public Data(string type, Attributes attrs)
        {
            Type = type;
            Attributes = attrs;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("attributes")]
        public Attributes Attributes { get; set; }
    }
}
