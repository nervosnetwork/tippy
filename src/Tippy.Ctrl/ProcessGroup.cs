using static System.Console;
using System.Diagnostics;

namespace Tippy.Ctrl
{
    class ProcessGroup
    {
        internal ProcessInfo ProcessInfo { get; private set; }
        Process.CommandProcess? node;
        Process.CommandProcess? miner;
        Process.CommandProcess? indexer;

        internal ProcessGroup(ProcessInfo processInfo)
        {
            ProcessInfo = processInfo;
        }

        internal event NodeLogEventHandler? NodeLogReceived;

        internal bool IsRunning => node?.IsRunning ?? false;

        internal void Start()
        {
            // TODO: update configuration before each launch
            WriteLine("Starting child processes...");
            if (node == null)
            {
                node = new Process.NodeProcess(ProcessInfo);
                node.LogReceived += new Process.LogEventHandler(OnLogReceived);
            }
            node.Start();
            // Wait for the RPC to get ready.
            // A better approach would be to catch ckb output to make sure it's already listening.
            System.Threading.Tasks.Task.Delay(1000).Wait();

            if (ProcessInfo.Chain == Core.Models.Project.ChainType.Dev)
            {
                if (miner == null)
                {
                    miner = new Process.MinerProcess(ProcessInfo);
                    miner.LogReceived += new Process.LogEventHandler(OnLogReceived);
                }
                miner.Start();
            }

            if (indexer == null)
            {
                indexer = new Process.IndexerProcess(ProcessInfo);
            }
            indexer.Start();
            WriteLine("Started child processes.");
        }

        internal void Stop()
        {
            WriteLine("Stopping child processes...");
            indexer?.Stop();
            miner?.Stop();
            node?.Stop();
            WriteLine("Stopped child processes.");
        }

        internal void Restart()
        {
            Stop();
            Start();
        }

        internal void ResetData()
        {
            Debug.Assert(!IsRunning);
            Process.NodeProcess np = new(ProcessInfo);
            np.Reset();
        }

        void OnLogReceived(object? sender, Process.LogEventArgs e)
        {
            WriteLine(e.Log);
            NodeLogReceived?.Invoke(sender, new LogReceivedEventArgs() { Log = "\n" + e.Log });
        }
    }
}
