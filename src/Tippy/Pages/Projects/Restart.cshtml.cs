using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.Ctrl;
using Tippy.Core.Models;
using Microsoft.AspNetCore.Http;

namespace Tippy.Pages.Projects
{
    public class RestartModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public RestartModel(Tippy.Core.Data.DbContext context)
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
                ProcessManager.Restart(Project);
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
