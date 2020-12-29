using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Tippy.Pages.Home
{
    public class SwitchProjectModel : PageModelBase
    {
        public SwitchProjectModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var project = await DbContext.Projects.FindAsync(id);
            if (project == null)
            {
                return NotFound();
            }

            var projects = await DbContext.Projects.ToListAsync();
            projects.ForEach(p => p.IsActive = false);
            project.IsActive = true;
            await DbContext.SaveChangesAsync();

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
