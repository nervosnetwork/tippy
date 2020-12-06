using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace Tippy.Pages.Home
{
    public class SwitchProjectModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public SwitchProjectModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await _context.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var projects = await _context.Projects.ToListAsync();
            projects.ForEach(p => p.IsActive = false);
            project.IsActive = true;
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
