using System;
using System.Collections.Generic;
using Tippy.Core.Models;

namespace Tippy.Pages.RecordedTransactions
{
    public class IndexModel : PageModelBase
    {
        public List<RecordedTransaction> RecordedTransactions { get; set; } = new List<RecordedTransaction>();

        public IndexModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public void OnGet()
        {
            DbContext.Entry(ActiveProject!).Collection(p => p.RecordedTransactions).Load();
            RecordedTransactions = ActiveProject!.RecordedTransactions;
            RecordedTransactions.Reverse();
        }
    }
}
