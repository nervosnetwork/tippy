using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Timers;
using Ckb.Rpc;
using Tippy.Core.Models;
using static System.Console;

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

        private Timer? advancedMiningTimer;
        private int remainingBlocksToMine = 0;
        internal List<DeniedTransaction>? deniedTransactions;

        internal bool IsRunning => node?.IsRunning ?? false;
        internal bool IsMinerRunning => miner?.IsRunning ?? false;
        internal bool IsAdvancedMinerRunning => advancedMiningTimer != null;
        internal bool CanStartMining => IsRunning && !(IsMinerRunning || IsAdvancedMinerRunning);

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
            node.Start();
            // Wait for the RPC to get ready.
            // A better approach would be to catch ckb output to make sure it's already listening.
            System.Threading.Tasks.Task.Delay(1000).Wait();

            // Do not start miner automatically.

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
            string? blockHash;
            if (deniedTransactions?.Count > 0)
            {
                var proposeList = new HashSet<string>(deniedTransactions
                    .Where(d => d.DenyType == DeniedTransaction.Type.Propose)
                    .Select(d => d.TxHash.Substring(0, 22))); // Short IDs, like 0x4dc1b9d993638e4ae212
                var commitList = new HashSet<string>(deniedTransactions.
                    Where(d => d.DenyType == DeniedTransaction.Type.Commit)
                    .Select(d => d.TxHash));
                var template = rpc.GetBlockTemplate()!;
                template.Proposals = (string[])template.Proposals.Where(p => !proposeList.Contains(p));
                template.Transactions = (Ckb.Types.TransactionTemplate[])template.Transactions.Where(t => !commitList.Contains(t.Hash));
                blockHash = rpc.GenerateBlockWithTemplate(template);
            }
            else
            {
                blockHash = rpc.GenerateBlock();
            }
            WriteLine($"Generated block {blockHash}");
        }

        // Advanced mining
        internal void StartAdvancedMining(int blocks, int interval)
        {
            if (ProcessInfo.Chain != Core.Models.Project.ChainType.Dev || !IsRunning)
            {
                return;
            }

            if (IsAdvancedMinerRunning)
            {
                return;
            }

            advancedMiningTimer = new(interval * 1000);
            advancedMiningTimer.Elapsed += OnMiningNextBlock;
            advancedMiningTimer.AutoReset = true;
            advancedMiningTimer.Enabled = true;
            remainingBlocksToMine = blocks;

            WriteLine($"Generating {blocks} blocks...");
        }

        internal void StopAdvancedMining()
        {
            if (advancedMiningTimer != null)
            {
                advancedMiningTimer.Stop();
                advancedMiningTimer.Dispose();
                advancedMiningTimer = null;
                WriteLine($"Generated blocks.");
            }
        }

        private void OnMiningNextBlock(Object source, ElapsedEventArgs e)
        {
            MineOneBlock();
            remainingBlocksToMine -= 1;
            if (remainingBlocksToMine == 0)
            {
                StopAdvancedMining();
            }
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
