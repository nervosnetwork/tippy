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
    public class GenerateBlockModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public GenerateBlockModel(Tippy.Core.Data.DbContext context)
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
                try
                {
                    ProcessManager.StartMiner(Project, ProcessManager.MinerMode.SingleBlock);
                }
                catch (System.InvalidOperationException e)
                {
                    TempData["ErrorMessage"] = e.Message;
                }
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
