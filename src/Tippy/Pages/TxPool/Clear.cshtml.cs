using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.TxPool
{
    public class ClearModel : PageModelBase
    {
        public ClearModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public IActionResult OnPost()
        {
            if (ActiveProject != null)
            {
                Rpc().ClearTxPool();
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}

