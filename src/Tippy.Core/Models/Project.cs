using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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

        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Chain Type")]
        public ChainType Chain { get; set; } = ChainType.Dev;

        [Display(Name = "RPC Port")]
        [Required]
        [Range(6000, 65535, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int NodeRpcPort { get; set; }

        [Display(Name = "Network Port")]
        [Required]
        [Range(6000, 65535, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int NodeNetworkPort { get; set; }


        [Display(Name = "InitCapacity")]
        [Required]
        [Range(1, 18446744073709551615, ErrorMessage = "{0} must be between {1} and {2}.")]
        public UInt64 InitCapacity { get; set; } = 2000000000000000000;









        [Display(Name = "Indexer RPC Port")]
        [Required]
        [Range(6000, 65535, ErrorMessage = "{0} must be between {1} and {2}.")]
        public int IndexerRpcPort { get; set; }

        [Display(Name = "Block Assembler Lock Arg")]
        [Required]
        public string LockArg { get; set; } = "0x";

        [Display(Name = "Active")]
        public bool IsActive { get; set; }

        public string ExtraToml { get; set; } = "";

        public List<Token> Tokens { get; set; } = default!;
        public List<RecordedTransaction> RecordedTransactions { get; set; } = default!;
        public List<DeniedTransaction> DeniedTransactions { get; set; } 


        
        
        public  List<Contracts> Contracts { get; set; } = default!;


    }
}
