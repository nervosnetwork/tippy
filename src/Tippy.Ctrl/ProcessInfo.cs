using Tippy.Core.Models;

namespace Tippy.Ctrl
{
    public record ProcessInfo(int ID, Project.ChainType Chain, int NodeRpcPort, int NodeNetworkPort,
        int IndexerRpcPort, string LockArg, string ExtraToml)
    {
        public static ProcessInfo FromProject(Project project) =>
            new(project.Id, project.Chain, project.NodeRpcPort, project.NodeNetworkPort,
                project.IndexerRpcPort, project.LockArg, project.ExtraToml);
    }
}
