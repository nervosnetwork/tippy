using System.Text.Json.Serialization;
using System.Linq;

using DefaultAttributesType = System.Collections.Generic.Dictionary<string, object>;

namespace Tippy.ApiData
{
    public class Result<T>
    {
        public Result()
        {
        }

        public Result(Data<T> data)
        {
            Data = data;
        }

        public Result(string type, T attrs)
        {
            Data = new(type, attrs);
        }

        [JsonPropertyName("data")]
        public Data<T> Data { get; set; }
    }

    public class ArrayResult<T>
    {
        public ArrayResult(string type, T[] attrs)
        {
            Data = attrs.Select(attr => new Data<T>(type, attr)).ToArray();
        }

        [JsonPropertyName("data")]
        public Data<T>[] Data { get; set; }
    }

    public class Result : Result<DefaultAttributesType>
    {
        public Result() : base() { }
        public Result(Data data) : base(data) { }
    }

    public class Data<T>
    { 
        public Data()
        {
        }

        public Data(string type, T attrs)
        {
            Type = type;
            Attributes = attrs;
        }

        [JsonPropertyName("id")]
        public string Id { get; set; } = "1";

        [JsonPropertyName("type")]
        public string Type { get; set; } = "";

        [JsonPropertyName("attributes")]
        public T Attributes { get; set; }
    }

    public class Data : Data<DefaultAttributesType>
    {
        public Data() : base() { }

        public Data(string type, DefaultAttributesType attrs) : base(type, attrs) { }
    }
}
