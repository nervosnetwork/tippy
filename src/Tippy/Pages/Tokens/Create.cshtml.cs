using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;
using Tippy.Helpers;

namespace Tippy.Pages.Tokens
{
    public class CreateModel : PageModelBase
    {
        public CreateModel(Tippy.Core.Data.TippyDbContext context) : base(context)
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
            Token.Hash = ScriptHelper.ComputeHash(
                new Script
                {
                    CodeHash = Token.TypeScriptCodeHash,
                    Args = Token.TypeScriptArgs,
                    HashType = Token.TypeScriptHashType
                });
            DbContext.Tokens.Add(Token);
            await DbContext.SaveChangesAsync();

            return RedirectToPage("./Details", new { id = Token.Id });
        }
    }
}
