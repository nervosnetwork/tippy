using System;

namespace Tippy.Core.Models
{
    public class Project
    {
        public enum ChainType
        {
            Mainnet,
            Testnet,
            Dev
        }

        public int ID { get; set; }
        public string Name { get; set; }
        public ChainType Chain { get; set; }
        public string NodeRpcPort { get; set; }
        public string NodeNetworkPort { get; set; }
        public string IndexerRpcPort { get; set; }
    }
}
