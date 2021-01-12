using System;
using Ckb.Rpc;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.Transactions
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public String TxHash = "";

        public IActionResult OnGet(string? txhash)
        {
            if (txhash == null)
            {
                return NotFound();
            }

            if (ActiveProject == null)
            {
                return NotFound();
            }

            TxHash = txhash;

            Client client = new($"http://localhost:{ActiveProject.NodeRpcPort}");

            // TODO

            return Page();
        }
    }
}
