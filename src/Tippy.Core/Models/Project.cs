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
        [Range(6000, 65535, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int NodeRpcPort { get; set; }

        [Display(Name = "Network Port")]
        [Required]
        [Range(6000, 65535, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int NodeNetworkPort { get; set; }

        [Display(Name = "Indexer RPC Port")]
        [Required]
        [Range(6000, 65535, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int IndexerRpcPort { get; set; }

        [Display(Name = "Block Assembler Lock Arg")]
        [Required]
        public string LockArg { get; set; }

        [Display(Name = "Active")]
        public bool IsActive { get; set; }
    }
}
