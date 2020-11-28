using Tippy.Core.Models;

namespace Tippy.Ctrl
{
    public record ProcessInfo(int ID, Project.ChainType Chain, int NodeRpcPort,
        int NodeNetworkPort, int IndexerRpcPort, string LockArg)
    {
        public static ProcessInfo FromProject(Project project) =>
            new ProcessInfo(project.ID, project.Chain, project.NodeRpcPort,
                project.NodeNetworkPort, project.IndexerRpcPort, project.LockArg);
    }
}
