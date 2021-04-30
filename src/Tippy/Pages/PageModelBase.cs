using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages
{
    /// <summary>
    /// Base page model for pages. Get pages should have page model inherited from this
    /// to have common properties used by layout. Post pages may decide if they should
    /// use inherited page model or not.
    /// </summary>
    public class PageModelBase : PageModel
    {
        protected readonly Tippy.Core.Data.TippyDbContext DbContext;

        public IList<Project> Projects { get; set; } = new List<Project>();
        public Project? ActiveProject { get; set; }
        public UInt64 TipBlockNumber { get; set; }
        public EpochView? EpochView { get; set; }
        public String ProcessInfo { get; set; } = "";
        public bool IsDebuggerSupported = !OperatingSystem.IsWindows();
        public bool DebuggerDepsInstalled { get; set; } = false;
        public Dictionary<string, Token> Tokens { get; set; } = new(); // Token.Hash -> Token

        protected PageModelBase(Tippy.Core.Data.TippyDbContext context)
        {
            DbContext = context;
        }

        protected bool IsMainnet() => ActiveProject?.Chain == Project.ChainType.Mainnet;
        protected string AddressPrefix() => IsMainnet() ? "ckb" : "ckt";

        protected Client Rpc() => new Client($"http://localhost:{ActiveProject!.NodeRpcPort}");
        protected IndexerClient IndexerRpc() => new IndexerClient($"http://localhost:{ActiveProject!.IndexerRpcPort}");

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            ProcessInfo = ProcessManager.Info;
            DebuggerDepsInstalled = ProcessManager.DebuggerDepsInstalled;

            Projects = await DbContext.Projects.Include(p => p.Tokens).ToListAsync();
            ActiveProject = await DbContext.Projects.FirstOrDefaultAsync(p => p.IsActive);
            if (ActiveProject == null && Projects.Count > 0)
            {
                ActiveProject = Projects[0]; // Fall back to none active project
            }

            if (ActiveProject != null && ProcessManager.IsRunning(ActiveProject))
            {
                foreach (var token in ActiveProject.Tokens)
                {
                    if (!Tokens.ContainsKey(token.Hash))
                    {
                        Tokens.Add(token.Hash, token);
                    }
                }

                Client rpc = Rpc();
                try
                {
                    EpochView = rpc.GetCurrentEpoch();
                    TipBlockNumber = rpc.GetTipBlockNumber();
                }
                catch
                {
                    // CKB node not respoding yet. Set default values.
                    EpochView = new EpochView()
                    {
                        Number = "0x0",
                        StartNumber = "0x0",
                        Length = "0x0",
                        CompactTarget = "0x0",
                    };
                    TipBlockNumber = 0;
                }
            }

            await next();
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
