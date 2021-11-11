using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Projects
{
    public class StartModel : PageModel
    {
        private readonly Tippy.Core.Data.TippyDbContext _context;

        public StartModel(Tippy.Core.Data.TippyDbContext context)
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

            Project = _context.Projects.Include(p => p.Contracts).Where(p => p.Id == id).FirstOrDefault();
                //await _context.Projects.Include.FindAsync(id);

            if (Project != null)
            {
                try
                {
                    ProcessManager.Start(Project);
                }
                catch (System.InvalidOperationException e)
                {
                    TempData["ErrorMessage"] = e.Message;
                }
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            if (!referer.Contains("projects"))
            {
                return Redirect(referer);
            }
            return RedirectToPage("./Index");
        }
    }
}
