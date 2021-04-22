using System;
using System.Collections.Generic;
using Tippy.Core.Models;
using Tippy.Ctrl.Process;

namespace Tippy.Ctrl
{
    public class LogReceivedEventArgs : EventArgs
    {
        public int ID { get; init; }
        public string? Log { get; init; }
    }

    public delegate void NodeLogEventHandler(object? sender, LogReceivedEventArgs e);

    public class ProcessManager
    {
        public static event NodeLogEventHandler? NodeLogReceived;

        /// <summary>
        /// CKB related binaries version info
        /// </summary>
        public static string Info { get; private set; } = "";

        public enum MinerMode
        {
            Default,
            SingleBlock,
            Sophisticated,
        }

        static List<ProcessGroup> processGroups = new List<ProcessGroup>();

        static ProcessGroup? GroupFor(Project project) =>
            processGroups.Find(g => g.ProcessInfo == ProcessInfo.FromProject(project));

        public static bool IsRunning(Project project) => project != null && GroupFor(project) != null;
        public static bool IsMinerRunning(Project project)
        {
            if (project != null && GroupFor(project) is ProcessGroup group)
            {
                return group.IsMinerRunning;
            }
            return false;
        }
        public static bool IsAdvancedMinerRunning(Project project)
        {
            if (project != null && GroupFor(project) is ProcessGroup group)
            {
                return group.IsAdvancedMinerRunning;
            }
            return false;
        }
        public static bool CanStartMining(Project project)
        {
            if (project != null && GroupFor(project) is ProcessGroup group)
            {
                return group.CanStartMining;
            }
            return false;
        }

        public static string GetLogFolder(Project project)
        {
            ProcessGroup group = new(ProcessInfo.FromProject(project));
            return group.LogFolder();
        }

        public static void FetchInfo()
        {
            BinariesInfo binariesInfo = new();
            binariesInfo.Refresh();
            Info = binariesInfo.Info;
        }

        /// If any port is already in use, throw InvalidOperationException.
        public static void Start(Project project)
        {
            if (!IsRunning(project))
            {
                ProcessGroup group = new(ProcessInfo.FromProject(project));
                var portsInUse = group.PortsInUse();
                if (portsInUse.Count > 0)
                {
                    var message = $"Port(s) {string.Join(", ", portsInUse)} already used. Please update project to use other ports.";
                    throw new System.InvalidOperationException(message);
                }

                group.NodeLogReceived += OnLogReceived;
                group.Start();
                processGroups.Add(group);
            }
        }

        public static void Stop()
        {
            foreach (var group in processGroups)
            {
                group.Stop();
            }
            processGroups.Clear();
        }

        public static void Stop(Project project)
        {
            var group = GroupFor(project);
            if (group == null)
            {
                return;
            }
            group.Stop();
            group.NodeLogReceived -= OnLogReceived;
            processGroups.Remove(group);
        }

        public static void Restart(Project project)
        {
            Stop(project);
            Start(project);
        }

        // Note: project should have DeniedTransactions loaded before calling this.
        public static void StartMiner(Project project, MinerMode mode, int blocks = 1, int interval = 1)
        {
            var group = GroupFor(project);
            if (group == null)
            {
                return;
            }
            group.deniedTransactions = project.DeniedTransactions;

            if (mode == MinerMode.Default)
            {
                group.StartMiner();
            }
            else if (mode == MinerMode.SingleBlock)
            {
                group.MineOneBlock();
            }
            else if (mode == MinerMode.Sophisticated)
            {
                group.StartAdvancedMining(blocks, interval);
            }
        }

        public static void StopMiner(Project project)
        {
            var group = GroupFor(project);
            group?.StopMiner();
        }

        public static void ResetData(Project project)
        {
            Stop(project);
            ProcessGroup group = new(ProcessInfo.FromProject(project));
            group.ResetData();
        }

        static void OnLogReceived(object? sender, LogReceivedEventArgs e)
        {
            Console.WriteLine(e.Log);
            if (sender is ProcessGroup group)
            {
                NodeLogReceived?.Invoke(sender, e);
            }
        }
    }
}
