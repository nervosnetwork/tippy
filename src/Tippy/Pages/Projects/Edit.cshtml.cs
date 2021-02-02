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
        public EditModel(Tippy.Core.Data.DbContext context) : base(context)
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

            Project = await DbContext.Projects.FindAsync(id);
            if (Project == null)
            {
                return NotFound();
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            var toUpdate = await DbContext.Projects.FindAsync(id);
            if (toUpdate == null)
            {
                return NotFound();
            }

            if (await TryUpdateModelAsync<Project>(
                toUpdate,
                "project",
                p => p.Name, p => p.NodeRpcPort, p => p.NodeNetworkPort, p => p.IndexerRpcPort, p => p.LockArg))
            {
                ProcessManager.Stop(toUpdate);

                if (toUpdate.LockArg.StartsWith("ckb") || toUpdate.LockArg.StartsWith("ckt"))
                {
                    toUpdate.LockArg = Address.ParseAddress(toUpdate.LockArg, toUpdate.LockArg.Substring(0, 3)).Args;
                }
                await DbContext.SaveChangesAsync();
                return RedirectToPage("./Index");
            }

            return Page();
        }
    }
}
