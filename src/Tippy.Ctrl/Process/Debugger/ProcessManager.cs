using Tippy.Core.Models;

namespace Tippy.Ctrl.Process.Debugger
{
    public class ProcessManager
    {
        private static GdbProcess? GdbProcessInstance;
        private static DebuggerProcess? DebuggerProcessInstance;

        public static void Start(Project project, string scriptGroupType, string scriptHash, string txFilePath, string debugFilePath, string ioType, int ioIndex, string scriptVersion, string? binaryForDebugger)
        {
            Stop();
            ProcessInfo processInfo = ProcessInfo.FromProject(project);
            GdbProcessInstance = new GdbProcess(processInfo, debugFilePath);
            DebuggerProcessInstance = new DebuggerProcess(processInfo, scriptGroupType, scriptHash, txFilePath, ioType, ioIndex, scriptVersion, binaryForDebugger);
            DebuggerProcessInstance.Start();
            GdbProcessInstance.Start();
        }

        public static void Stop()
        {
            GdbProcessInstance?.Stop();
            DebuggerProcessInstance?.Stop();
        }

        public static void SetEnv()
        {
            // BUGBUG: .Net won't find commands on M1 Mac which has homebrew location at `/opt/homebrew/bin`,
            // or on Linux which at `~/.linuxbrew/bin`.
            var path = System.Environment.GetEnvironmentVariable("PATH") ?? "";
            if (!string.IsNullOrEmpty(path))
            {
                path += ":";
            }
            path += "/opt/homebrew/bin:/home/linuxbrew/.linuxbrew/bin:~/.linuxbrew/bin";
            System.Environment.SetEnvironmentVariable("PATH", path);
        }
    }
}
