using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;

namespace Tippy.Core.Models
{
    [Index(nameof(TxHash), nameof(DenyType), IsUnique = true)]
    public class DeniedTransaction
    {
        public enum Type
        {
            Propose,
            Commit
        }

        public int Id { get; set; }

        [Display(Name = "Disallow from")]
        public string TxHash { get; set; } = "";

        [Display(Name = "Disallow from")]
        public Type DenyType { get; set; } = Type.Commit;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
