using System.Linq;
using System.Threading.Tasks;
using Ckb.Address;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Projects
{
    public class EditModel : PageModelBase
    {
        public EditModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project = await DbContext.Projects
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.Id == id);
            if (Project == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var toUpdate = await DbContext.Projects.FirstOrDefaultAsync(p => p.Id == id);
            if (toUpdate == null)
            {
                return NotFound();
            }

            toUpdate.Name = Project.Name;
            toUpdate.NodeRpcPort = Project.NodeRpcPort;
            toUpdate.NodeNetworkPort = Project.NodeNetworkPort;
            toUpdate.IndexerRpcPort = Project.IndexerRpcPort;
            toUpdate.LockArg = Project.LockArg;
            if (toUpdate.LockArg.StartsWith("ckb") || toUpdate.LockArg.StartsWith("ckt"))
            {
                toUpdate.LockArg = Address.ParseAddress(toUpdate.LockArg, toUpdate.LockArg.Substring(0, 3)).Args;
            }

            ProcessManager.Stop(toUpdate);

            await DbContext.SaveChangesAsync();
            return RedirectToPage("./Index");
        }
    }
}
