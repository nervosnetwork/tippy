using static System.Console;
using System.Diagnostics;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using Ckb.Rpc;

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
        internal bool IsMinerRunning => miner?.IsRunning ?? false;

        internal string LogFolder()
        {
            Process.NodeProcess p = new(ProcessInfo);
            return Path.Combine(p.WorkingDirectory(), "data", "logs");
        }

        internal void Start()
        {
            WriteLine("Starting child processes...");
            if (node == null)
            {
                node = new Process.NodeProcess(ProcessInfo);
                node.LogReceived += OnLogReceived;
            }
            //node.Start();
            // Wait for the RPC to get ready.
            // A better approach would be to catch ckb output to make sure it's already listening.
            System.Threading.Tasks.Task.Delay(1000).Wait();

            // Do not start miner automatically.

            if (indexer == null)
            {
                indexer = new Process.IndexerProcess(ProcessInfo);
            }
            //indexer.Start();
            WriteLine("Started child processes.");
        }

        internal void Stop()
        {
            WriteLine("Stopping child processes...");
            StopMiner();
            indexer?.Stop();
            node?.Stop();
            WriteLine("Stopped child processes.");
        }

        internal void Restart()
        {
            Stop();
            Start();
        }

        internal void StartMiner()
        {
            if (ProcessInfo.Chain != Core.Models.Project.ChainType.Dev || !IsRunning)
            {
                return;
            }

            WriteLine("Starting miner process...");
            if (miner == null)
            {
                miner = new Process.MinerProcess(ProcessInfo);
                miner.LogReceived += OnLogReceived;
            }
            miner.Start();
            WriteLine("Started miner process.");
        }

        internal void StopMiner()
        {
            WriteLine("Stopping miner process...");
            miner?.Stop();
            WriteLine("Stopped miner process.");
        }

        internal void MineOneBlock()
        {
            if (ProcessInfo.Chain != Core.Models.Project.ChainType.Dev || !IsRunning)
            {
                return;
            }

            WriteLine("Generating block...");
            Client rpc = new($"http://localhost:{ProcessInfo.NodeRpcPort}");
            var block = rpc.GenerateBlock();
            WriteLine($"Generated block {block}");
        }

        internal List<int> PortsInUse()
        {
            var allPortsInUse = Util.LocalPort.PortsInUse();
            var portsToCheck = new int[]
                {
                    ProcessInfo.NodeRpcPort,
                    ProcessInfo.NodeNetworkPort,
                    ProcessInfo.IndexerRpcPort
                };
            return portsToCheck
                .Where(p => allPortsInUse.Contains(p))
                .ToList();
        }

        internal void ResetData()
        {
            Debug.Assert(!IsRunning);
            Process.NodeProcess np = new(ProcessInfo);
            np.Reset();
        }

        void OnLogReceived(object? sender, LogReceivedEventArgs e)
        {
            NodeLogReceived?.Invoke(this, e);
        }
    }
}
