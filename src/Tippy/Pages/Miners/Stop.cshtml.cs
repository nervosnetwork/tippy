using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Miners
{
    public class StopModel : PageModel
    {
        private readonly Tippy.Core.Data.TippyDbContext _context;

        public StopModel(Tippy.Core.Data.TippyDbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project? Project { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project = await _context.Projects.FindAsync(id);

            if (Project != null)
            {
                ProcessManager.StopMiner(Project);
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
