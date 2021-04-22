using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.DeniedTransactions
{
    public class CreateModel : PageModelBase
    {
        public CreateModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnPostAsync(string hash, [FromQuery(Name = "type")] string type)
        {
            // TODO
            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
