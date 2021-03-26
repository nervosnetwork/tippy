using System;
using System.Collections.Generic;
using Tippy.Core.Models;

namespace Tippy.Pages.FailedTransactions
{
    public class IndexModel : PageModelBase
    {
        public List<FailedTransaction> FailedTransactions { get; set; } = new List<FailedTransaction>();

        public IndexModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public void OnGet()
        {
            DbContext.Entry(ActiveProject!).Collection(p => p.FailedTransactions).Load();
            FailedTransactions = ActiveProject!.FailedTransactions;
            FailedTransactions.Reverse();
        }
    }
}
