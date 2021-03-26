using System;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.ApiData;
using Tippy.Core.Models;
using static Tippy.Helpers.TransactionHelper;

namespace Tippy.Pages.FailedTransactions
{
    public class DetailsModel : PageModelBase
    {
        public FailedTransaction Transaction { get; set; } = default!;
        public string RawTransaction { get; set; } = "{}";

        public Ckb.Types.Transaction? ParsedTransaction { get; set; } = null;
        public DisplayInput[] DisplayInputs = Array.Empty<DisplayInput>();
        public DisplayOutput[] DisplayOutputs = Array.Empty<DisplayOutput>();

        public int ID;

        public DetailsModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            ID = (int)id;

            Transaction = await DbContext.FailedTransactions.FirstOrDefaultAsync(t => t.Id == id);

            if (Transaction == null)
            {
                return NotFound();
            }

            ParseRawTransaction();
            SetParsedTransaction();

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

        private void SetParsedTransaction()
        {
            try
            {
                var tx = Ckb.Types.Transaction.FromJson(Transaction.RawTransaction);

                Ckb.Rpc.Client client = Rpc();
                string prefix = IsMainnet() ? "ckb" : "ckt";
                var previousOutputs = GetPreviousOutputs(client, tx.Inputs);
                var (displayInputs, displayOutputs) = GenerateNotCellbaseDisplayInfos(tx.Inputs, tx.Outputs, previousOutputs, prefix, "");
                ParsedTransaction = tx;
                DisplayInputs = displayInputs;
                DisplayOutputs = displayOutputs;
            }
            catch
            {
                ParsedTransaction = null;
            }
        }
    }
}
