using System;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.Home
{
    public class EpochInfoModel : PageModelBase
    {
        public EpochInfoModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public PartialViewResult OnGet()
        {
            return Partial("_EpochInfo", this);
        }
    }
}
