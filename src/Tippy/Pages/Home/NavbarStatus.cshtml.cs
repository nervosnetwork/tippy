using System;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.Home
{
    public class NavbarStatusModel : PageModelBase
    {
        public NavbarStatusModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public PartialViewResult OnGet()
        {
            return Partial("Shared/_NavbarStatus", this);
        }
    }
}
