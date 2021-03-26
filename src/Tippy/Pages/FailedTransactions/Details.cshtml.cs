using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;

namespace Tippy.Pages.FailedTransactions
{
    public class DetailsModel : PageModelBase
    {
        public FailedTransaction Transaction { get; set; } = default!;
        public string RawTransaction { get; set; } = "{}";
        public DetailsModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Transaction = await DbContext.FailedTransactions.FirstOrDefaultAsync(t => t.Id == id);

            if (Transaction == null)
            {
                return NotFound();
            }

            ParseRawTransaction();

            return Page();
        }

        private void ParseRawTransaction()
        {
            try
            {
                var obj = JsonSerializer.Deserialize<object>(Transaction.RawTransaction);
                RawTransaction = JsonSerializer.Serialize(
                    obj,
                    new JsonSerializerOptions
                    {
                        WriteIndented = true,
                    });
            }
            catch
            {
                RawTransaction = Transaction.RawTransaction;
            }
        }
    }
}
