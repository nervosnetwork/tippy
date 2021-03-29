using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;

namespace Tippy.Pages.Tokens
{
    public class DeleteModel : PageModelBase
    {
        public DeleteModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [BindProperty]
        public Token Token { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Token = await DbContext.Tokens.FindAsync(id);

            if (Token == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Token = await DbContext.Tokens.FindAsync(id);

            if (Token != null)
            {
                DbContext.Tokens.Remove(Token);
                await DbContext.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
