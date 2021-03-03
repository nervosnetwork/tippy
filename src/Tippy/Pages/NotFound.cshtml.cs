using System;

namespace Tippy.Pages
{
    public class NotFoundModel : PageModelBase
    {
        public bool Is404 = false;

        public NotFoundModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public void OnGet(int statusCode)
        {
            Is404 = statusCode == 404;
        }
    }
}
