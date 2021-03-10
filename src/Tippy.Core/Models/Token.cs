using System;
using System.ComponentModel.DataAnnotations;

namespace Tippy.Core.Models
{
    public class Token
    {
        public int Id { get; set; }
        [Required]

        public string Name { get; set; } = string.Empty;

        public string Symbol { get; set; } = string.Empty;

        public string TypeScriptCodeHash { get; set; } = string.Empty;
        public string TypeScriptArgs { get; set; } = string.Empty;
        public string TypeScriptHashType { get; set; } = "type";

        public string Hash { get; set; } = string.Empty; // Computed hash of type script

        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
