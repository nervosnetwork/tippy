using System;

namespace Tippy.Ctrl
{
    public class LogReceivedEventArgs : EventArgs
    { 
        public string? Log { get; internal set; }
    }

    public delegate void NodeLogEventHandler(object? sender, LogReceivedEventArgs e);

    public class ProcessManager
    {
        static Process.CommandProcess? node;
        static Process.CommandProcess? miner;
        static Process.CommandProcess? indexer;

        public static event NodeLogEventHandler? NodeLogReceived;

        public static bool IsRunning => node?.IsRunning ?? false;

        public static void UpdateConfiguration() => Process.NodeProcess.UpdateConfiguration();

        public static void Start()
        {
            Console.WriteLine("Starting child processes...");
            node ??= new Process.NodeProcess();
            node.Start();
            // Wait for the RPC to get ready.
            // A better approach would be to catch ckb output to make sure it's already listening.
            System.Threading.Tasks.Task.Delay(1000).Wait();

            miner ??= new Process.MinerProcess();
            miner.Start();

            indexer ??= new Process.IndexerProcess();
            indexer.Start();
            Console.WriteLine("Started child processes.");
        }

        public static void Stop()
        {
            Console.WriteLine("Stopping child processes...");
            indexer?.Stop();
            miner?.Stop();
            node?.Stop();
            Console.WriteLine("Stopped child processes.");
        }

        public static void Restart()
        {
            Stop();
            Start();
            // TODO: debug only. Remove this and connect to node process output.
            NodeLogReceived?.Invoke(null, new LogReceivedEventArgs() { Log = "Restarted..." });
        }

        public static void ResetData()
        {
            Stop();
            Process.NodeProcess.Reset();
            Start();
        }
    }
}
