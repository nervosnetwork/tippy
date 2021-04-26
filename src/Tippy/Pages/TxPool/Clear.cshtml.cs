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

        [TempData]
        public string Message { get; set; } = "";


        public IActionResult OnPost()
        {
            if (ActiveProject != null)
            {
                Rpc().ClearTxPool();
            }

            Message = "Tx Pool was cleared";

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}

