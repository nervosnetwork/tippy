using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Home
{
    public class QuickStartModel : PageModelBase
    {
        public QuickStartModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var calculatingFromUsed = Projects.Count > 0;
            var rpcPorts = Projects.Select(p => p.NodeRpcPort);
            var networkPorts = Projects.Select(p => p.NodeNetworkPort);
            var indexerPorts = Projects.Select(p => p.IndexerRpcPort);

            Project project = new()
            {
                Name = "CKB devchain",
                Chain = Project.ChainType.Dev,
                NodeRpcPort = calculatingFromUsed ? rpcPorts.Max() + 3 : 8114,
                NodeNetworkPort = calculatingFromUsed ? networkPorts.Max() + 3 : 8115,
                IndexerRpcPort = calculatingFromUsed ? indexerPorts.Max() + 3 : 8116,
                LockArg = "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7"
            };

            var projects = await DbContext.Projects.ToListAsync();
            projects.ForEach(p => p.IsActive = false);
            project.IsActive = true;
            DbContext.Projects.Add(project);
            await DbContext.SaveChangesAsync();
            try
            {
                ProcessManager.Start(project);
            }
            catch (System.InvalidOperationException e)
            {
                TempData["ErrorMessage"] = e.Message;
            }

            return RedirectToPage("/Home/Index");
        }
    }
}
