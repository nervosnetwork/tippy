using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Tippy.Core.Models
{
    public class FailedTransaction
    {
        public int Id { get; set; }

        public string RawTransaction { get; set; } = "";

        public string Error { get; set; } = "";

        [Required, DatabaseGenerated(DatabaseGeneratedOption.Computed)]
        public DateTime CreatedAt { get; set; }

        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
