using System;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;

namespace Tippy.Pages.Tokens
{
    public class DetailsModel : PageModelBase
    {
        public Token Token = default!;

        public DetailsModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public IActionResult OnGet(int id)
        {
            if (ActiveProject == null)
            {
                return NotFound();
            }

            var token = ActiveProject.Tokens.Find(t => t.Id == id);
            if (token == null)
            {
                return NotFound();
            }
            Token = token!;

            return Page();
        }
    }
}
