using System;
using Ckb.Address;
using Ckb.Rpc;
using Ckb.Types;
using Microsoft.AspNetCore.Mvc;
using Tippy.Ctrl;

namespace Tippy.Pages.Home
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.DbContext context) :base(context)
        {
        }

        public string MinerAddress { get; set; } = "";

        [TempData]
        public string Message { get; set; } = "";

        public bool IsNodeRunning { get; set; }
        public bool IsMinerRunning { get; set; }

        public void OnGet()
        {
            IsNodeRunning = ActiveProject != null && ProcessManager.IsRunning(ActiveProject);
            IsMinerRunning = IsNodeRunning && ProcessManager.IsMinerRunning(ActiveProject!);

            if (IsNodeRunning)
            {
                Client rpc = new ($"http://localhost:{ActiveProject!.NodeRpcPort}");
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
