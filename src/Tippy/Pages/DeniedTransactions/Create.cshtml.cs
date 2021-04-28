using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;

namespace Tippy.Pages.DeniedTransactions
{
    public class CreateModel : PageModelBase
    {
        public CreateModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [TempData]
        public string Message { get; set; } = "";

        public async Task<IActionResult> OnPostAsync(string hash, [FromQuery(Name = "type")] string type)
        {
            try
            {
                var item = new DeniedTransaction
                {
                    ProjectId = ActiveProject!.Id,
                    TxHash = hash,
                    DenyType = type == "propose" ? DeniedTransaction.Type.Propose : DeniedTransaction.Type.Commit
                };
                DbContext.DeniedTransactions.Add(item);
                await DbContext.SaveChangesAsync();
                Message = "Added to denylist.";
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
