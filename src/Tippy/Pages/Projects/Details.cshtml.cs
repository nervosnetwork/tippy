using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Data;
using Tippy.Core.Models;

namespace Tippy.Pages.Projects
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project = await DbContext.Projects.FirstOrDefaultAsync(m => m.Id == id);

            if (Project == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}
