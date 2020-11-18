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
            if (node == null)
            { 
                node = new Process.NodeProcess();
                node.LogReceived += new Process.LogEventHandler(OnLogReceived);
            }
            node.Start();
            // Wait for the RPC to get ready.
            // A better approach would be to catch ckb output to make sure it's already listening.
            System.Threading.Tasks.Task.Delay(1000).Wait();

            if (miner == null)
            { 
                miner = new Process.MinerProcess();
                miner.LogReceived += new Process.LogEventHandler(OnLogReceived);
            }
            miner.Start();

            if (indexer == null)
            { 
                indexer = new Process.IndexerProcess();
            }
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
        }

        public static void ResetData()
        {
            Stop();
            Process.NodeProcess.Reset();
            Start();
        }

        static void OnLogReceived(object? sender, Process.LogEventArgs e)
        {
            Console.WriteLine(e.Log);
            NodeLogReceived?.Invoke(sender, new LogReceivedEventArgs() { Log = "\n" + e.Log });
        }
    }
}
