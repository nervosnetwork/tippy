using System;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.Transactions
{
    public class MarkSudtModel : PageModelBase
    {
        public MarkSudtModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public IActionResult OnPost(string txhash, [FromQuery(Name = "index")] int index)
        {
            Client client = Rpc();
            TransactionWithStatus? transactionWithStatus = client.GetTransaction(txhash);
            // TODO: mark as sudt

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
