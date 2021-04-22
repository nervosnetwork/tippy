using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Miners
{
    public class AdvancedModel : PageModelBase
    {
        public bool IsMinerRunning = default;
        public bool CanStartMining = false;

        public AdvancedModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [BindProperty]
        public Project? Project { get; set; }

        [BindProperty]
        public int BlocksToGenerate { get; set; } = 5;

        [BindProperty]
        public int Interval { get; set; } = 3;

        public async Task<IActionResult> OnGetAsync(int id)
        {
            Project = await DbContext.Projects
                .Include(p => p.DeniedTransactions)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();
            if (Project == null)
            {
                return NotFound();
            }
            IsMinerRunning = ProcessManager.IsMinerRunning(Project);
            CanStartMining = ProcessManager.CanStartMining(Project);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            Project = await DbContext.Projects.FindAsync(id);
            if (Project != null)
            {
                try
                {
                    ProcessManager.StartMiner(Project, ProcessManager.MinerMode.Sophisticated, BlocksToGenerate, Interval);
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
