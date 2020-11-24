using System;
using System.Collections.Generic;
using Tippy.Core.Models;

namespace Tippy.Ctrl
{
    public class LogReceivedEventArgs : EventArgs
    { 
        public string? Log { get; init; }
    }

    public delegate void NodeLogEventHandler(object? sender, LogReceivedEventArgs e);

    public class ProcessManager
    {
        public static event NodeLogEventHandler? NodeLogReceived;

        static List<ProcessGroup> processGroups = new List<ProcessGroup>();

        static ProcessGroup? GroupFor(Project project) =>
            processGroups.Find(g => g.ProcessInfo == ProcessInfo.FromProject(project));

        public static bool IsRunning(Project project) => GroupFor(project) != null;

        public static void Start(Project project)
        {
            if (!IsRunning(project))
            {
                ProcessGroup group = new(ProcessInfo.FromProject(project));
                group.Start();
                // TODO connect log
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
            // TODO disconnect log
            group.Stop();
            processGroups.Remove(group);
        }

        public static void Restart(Project project)
        {
            Stop(project);
            Start(project);
        }

        public static void ResetData(Project project)
        {
            Stop(project);
            ProcessGroup group = new(ProcessInfo.FromProject(project));
            group.ResetData();
        }

        static void OnLogReceived(object? sender, Process.LogEventArgs e)
        {
            Console.WriteLine(e.Log);
            NodeLogReceived?.Invoke(sender, new LogReceivedEventArgs() { Log = "\n" + e.Log });
        }
    }
}
