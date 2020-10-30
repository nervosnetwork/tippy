using System;

namespace Tippy.Ctrl
{
    public class ProcessManager
    {
        static Process.CommandProcess node;
        static Process.CommandProcess miner;
        static Process.CommandProcess indexer;

        public static void Start()
        {
            Console.WriteLine("Starting child processes...");
            if (node == null)
            {
                node = new Process.NodeProcess();
            }
            node.Start();
            System.Threading.Tasks.Task.Delay(3000).Wait(); // Give change for the RPC to get ready

            if (miner == null)
            {
                miner = new Process.MinerProcess();
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
            indexer.Stop();
            miner.Stop();
            node.Stop();
            Console.WriteLine("Stopped child processes.");
        }
    }
}
