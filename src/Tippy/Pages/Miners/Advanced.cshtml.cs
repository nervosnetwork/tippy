using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Miners
{
    public class AdvancedModel : PageModelBase
    {
        public bool IsMinerRunning = default;

        public AdvancedModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        [BindProperty]
        public Project? Project { get; set; }
        public async Task<IActionResult> OnGetAsync(int id)
        {
            Project = await DbContext.Projects.FindAsync(id);
            if (Project == null)
            {
                return NotFound();
            }
            IsMinerRunning = ProcessManager.IsMinerRunning(Project);
            return Page();
        }
    }
}
