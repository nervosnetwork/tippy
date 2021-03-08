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
            if (transactionWithStatus != null)
            {
                Transaction tx = transactionWithStatus.Transaction;
                var data = tx.OutputsData[index];
                var typeScript = tx.Outputs[index].Type;
                // TODO: mark as sudt script
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
