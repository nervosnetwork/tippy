using System;
using Newtonsoft.Json;

namespace Ckb.Types.MockTransactionTypes
{
    public class MockInput
    {
        [JsonProperty(PropertyName = "input")]
        public Input Input { get; set; }

        [JsonProperty(PropertyName = "output")]
        public Output Output { get; set; }

        [JsonProperty(PropertyName = "data")]
        public String Data { get; set; }
    }

    public class MockCellDep
    {
        [JsonProperty(PropertyName = "cell_dep")]
        public CellDep CellDep { get; set; }

        [JsonProperty(PropertyName = "output")]
        public Output Output { get; set; }

        [JsonProperty(PropertyName = "data")]
        public String Data { get; set; }
    }

    public class MockInfo
    {
        [JsonProperty(PropertyName = "inputs")]
        public MockInput[] Inputs { get; set; }

        [JsonProperty(PropertyName = "cell_deps")]
        public MockCellDep[] CellDeps { get; set; }

        [JsonProperty(PropertyName = "header_deps")]
        public Header[] HeaderDeps { get; set; }
    }

    public class MockTransaction
    {
        [JsonProperty(PropertyName = "mock_info")]
        public MockInfo MockInfo { get; set; }

        [JsonProperty(PropertyName = "tx")]
        public Transaction Tx { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public static MockTransaction FromJson(string json)
        {
            var obj = JsonConvert.DeserializeObject<MockTransaction>(json);
            if (obj == null)
            {
                throw new Exception("Deserialize MockTransaction Failed!");
            }
            return obj;
        }
    }
}
