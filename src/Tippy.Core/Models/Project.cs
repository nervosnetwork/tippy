using System;
using System.ComponentModel.DataAnnotations;

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

        [Required]
        public string Name { get; set; }
        public ChainType Chain { get; set; } = ChainType.Dev;

        [Display(Name = "RPC Port")]
        [Required]
        public string NodeRpcPort { get; set; }

        [Display(Name = "Network Port")]
        [Required]
        public string NodeNetworkPort { get; set; }

        [Display(Name = "Indexer RPC Port")]
        [Required]
        public string IndexerRpcPort { get; set; }

        [Display(Name = "Block Assembler Lock Arg")]
        [Required]
        public string LockArg { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
