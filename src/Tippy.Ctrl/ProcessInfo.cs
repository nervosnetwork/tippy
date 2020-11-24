using Tippy.Core.Models;

namespace Tippy.Ctrl
{
    public record ProcessInfo
    {
        public int ID { get; init; }
        public Project.ChainType Chain { get; init; }
        public string NodeRpcPort { get; init; }
        public string NodeNetworkPort { get; init; }
        public string IndexerRpcPort { get; init; }
        public string LockArg { get; init; }

        public ProcessInfo(int id, Project.ChainType chain, string nodeRpcPort,
            string nodeNetworkPort, string indexerRpcPort, string lockArg)
        {
            ID = id;
            Chain = chain;
            NodeRpcPort = nodeRpcPort;
            NodeNetworkPort = nodeNetworkPort;
            IndexerRpcPort = indexerRpcPort;
            LockArg = lockArg;
        }

        public static ProcessInfo FromProject(Project project) =>
            new ProcessInfo(
                project.ID,
                project.Chain,
                project.NodeRpcPort,
                project.NodeNetworkPort,
                project.IndexerRpcPort,
                project.LockArg);
    }
}
