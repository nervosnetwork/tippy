using System;
using System.Collections.Generic;
using Tippy.Core.Models;

namespace Tippy.Pages.FailedTransactions
{
    public class IndexModel : PageModelBase
    {
        public List<RecordedTransaction> FailedTransactions { get; set; } = new List<RecordedTransaction>();

        public IndexModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public void OnGet()
        {
            DbContext.Entry(ActiveProject!).Collection(p => p.RecordedTransactions).Load();
            FailedTransactions = ActiveProject!.RecordedTransactions;
            FailedTransactions.Reverse();
        }
    }
}
