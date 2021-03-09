using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;

namespace Tippy.Pages.Tokens
{
    public class CreateModel : PageModelBase
    {
        public CreateModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        [BindProperty]
        public Token Token { get; set; } = default!;

        public void OnGet(string txhash, [FromQuery(Name = "index")] int index)
        {
            Client client = Rpc();
            TransactionWithStatus? transactionWithStatus = client.GetTransaction(txhash);
            if (transactionWithStatus != null)
            {
                Transaction tx = transactionWithStatus.Transaction;
                var data = tx.OutputsData[index];
                var typeScript = tx.Outputs[index].Type;
                Debug.Assert(typeScript != null);
                Token = new Token
                {
                    ProjectId = ActiveProject!.Id,
                    TypeScriptCodeHash = typeScript.CodeHash,
                    TypeScriptArgs = typeScript.Args,
                    TypeScriptHashType = typeScript.HashType
                };
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Token.Project = ActiveProject!;
            Token.Hash = "0x"; // TODO: compute type script hash
            DbContext.Tokens.Add(Token);
            await DbContext.SaveChangesAsync();

            return RedirectToPage("./Index", new { id = Token.Id });
        }
    }
}
