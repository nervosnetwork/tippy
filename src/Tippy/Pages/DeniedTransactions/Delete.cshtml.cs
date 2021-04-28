using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;

namespace Tippy.Pages.DeniedTransactions
{
    public class DeleteModel : PageModelBase
    {
        public DeleteModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [TempData]
        public string Message { get; set; } = "";

        public async Task<IActionResult> OnPostAsync(string hash, [FromQuery(Name = "type")] string type)
        {
            try
            {
                var denyType = type == "propose" ? DeniedTransaction.Type.Propose : DeniedTransaction.Type.Commit;
                var item = DbContext
                    .DeniedTransactions
                    .Where(t => t.TxHash == hash && t.DenyType == denyType)
                    .First();
                if (item != null)
                {
                    DbContext.DeniedTransactions.Remove(item);
                    await DbContext.SaveChangesAsync();
                    Message = "Removed from denylist.";
                }
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
