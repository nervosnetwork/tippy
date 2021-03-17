using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.Blocks
{
    public class RollBackToModel : PageModelBase
    {
        public RollBackToModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public IActionResult OnPost(string hash)
        {
            if (ActiveProject != null)
            {
                Rpc().Truncate(hash);
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
