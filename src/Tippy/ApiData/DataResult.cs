using System.Text.Json.Serialization;

namespace Tippy.ApiData
{
    public class DataResult
    {
        [JsonPropertyName("data")]
        public string Data { get; set; } = default!;
    }
}
