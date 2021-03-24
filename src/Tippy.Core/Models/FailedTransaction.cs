using System;

namespace Tippy.Core.Models
{
    public class FailedTransaction
    {
        public int Id { get; set; }

        public string RawTransaction { get; set; } = "";

        public string Error { get; set; } = "";

        public DateTime CreatedAt { get; set; } = default!;

        public int ProjectId { get; set; }
        public Project Project { get; set; } = default!;
    }
}
