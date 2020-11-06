using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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

        public Data(string type, Dictionary<string, object> attrs)
        {
            Type = type;
            Attributes = attrs;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("attributes")]
        public Dictionary<string, object> Attributes { get; set; }
    }
}
