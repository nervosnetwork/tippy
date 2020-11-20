using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Tippy.Core.Data;
using Tippy.Core.Models;

namespace Tippy.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public CreateModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            // TODO: pre-fill unused port numbers
            Project = new Project
            {
                Name = "Project name",
                Chain = Project.ChainType.Dev,
                LockArg = "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7"
            };
            return Page();
        }

        [BindProperty]
        public Project Project { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
