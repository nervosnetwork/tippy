using System;

namespace Tippy.Ctrl
{
    public class ProcessManager
    {
        static Process.CommandProcess node;
        static Process.CommandProcess miner;
        static Process.CommandProcess indexer;

        public static bool IsRunning
        {
            get
            {
                return node?.IsRunning ?? false;
            }
        }

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
            indexer.Stop();
            miner.Stop();
            node.Stop();
            Console.WriteLine("Stopped child processes.");
        }

        public static void Restart()
        {
            Stop();
            Start();
        }

        public static void UpdateConfiguration()
        {
            Process.NodeProcess.UpdateConfiguration();
        }
    }
}
