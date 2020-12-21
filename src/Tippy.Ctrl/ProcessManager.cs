using System;
using System.Collections.Generic;
using Tippy.Core.Models;

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

        public enum MinerMode
        {
            Default,
            SingleBlock,
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

        public static string GetLogFolder(Project project)
        {
            ProcessGroup group = new(ProcessInfo.FromProject(project));
            return group.LogFolder();
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
                    var message = $"Port(s) {string.Join(", " , portsInUse)} already used. Please update project to use other ports.";
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

        public static void StartMiner(Project project, MinerMode mode)
        {
            var group = GroupFor(project);
            if (group == null)
            {
                return;
            }

            if (mode == MinerMode.Default)
            {
                group.StartMiner();
            }
            else if (mode == MinerMode.SingleBlock)
            {
                // TODO: call rpc to generate one block
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
