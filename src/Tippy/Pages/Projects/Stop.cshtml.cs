using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Projects
{
    public class StopModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public StopModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Project Project { get; set; }

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
                ProcessManager.Stop(Project);
            }

            return RedirectToPage("./Index");
        }
    }
}
