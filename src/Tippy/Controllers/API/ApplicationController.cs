using Ckb.Rpc;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;

namespace Tippy.Controllers.API
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class ApplicationController : ControllerBase
    {
        protected Client? Rpc()
        {
            Project? activeProject = CurrentRunningProject();
            if (activeProject != null)
            {
                return new Client($"http://localhost:{activeProject.NodeRpcPort}");
            }

            return null;
        }

        protected bool IsMainnet()
        {
            return CurrentRunningProject()?.Chain == Project.ChainType.Mainnet;
        }

        private Project? CurrentRunningProject()
        {
            Project? activeProject = HttpContext.Items["ActiveProject"] as Project;
            if (activeProject != null && ProcessManager.IsRunning(activeProject))
            {
                return activeProject;
            }
            return null;
        }
    }
}
