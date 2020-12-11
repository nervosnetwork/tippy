using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Ckb.Address;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;

namespace Tippy.Pages.Home
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class IndexModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public IndexModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public IList<Project> Projects { get; set; } = new List<Project>();
        public Project? ActiveProject { get; set; }
        public UInt64 TipBlockNumber { get; set; }
        public string MinerAddress { get; set; } = "";
        public EpochView? EpochView { get; set; }

        [TempData]
        public string Message { get; set; } = "";

        public bool IsNodeRunning { get; set; }

        public async Task OnGetAsync()
        {
            Projects = await _context.Projects.ToListAsync();
            ActiveProject = HttpContext.Items["ActiveProject"] as Project;
            IsNodeRunning = ActiveProject != null && ProcessManager.IsRunning(ActiveProject);

            if (IsNodeRunning)
            {
                Client rpc = new ($"http://localhost:{ActiveProject!.NodeRpcPort}");
                EpochView = rpc.GetCurrentEpoch();
                TipBlockNumber = rpc.GetTipBlockNumber();
                MinerAddress = Address.GenerateAddress(
                    new Script
                    {
                        Args = ActiveProject.LockArg,
                        CodeHash = Address.SecpCodeHash,
                        HashType = Address.SecpHashType 
                    },
                    "ckt");
            }
        }
    }
}
