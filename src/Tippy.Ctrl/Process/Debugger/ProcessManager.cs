using Tippy.Core.Models;

namespace Tippy.Ctrl.Process.Debugger
{
    public class ProcessManager
    {
        private static GdbProcess? GdbProcessInstance;
        private static DebuggerProcess? DebuggerProcessInstance;

        public static void Start(Project project, string scriptGroupType, string scriptHash, string txFilePath, string debugFilePath, string ioType, int ioIndex, string? binaryForDebugger)
        {
            Stop();
            // TODO: Maybe not need project.
            ProcessInfo processInfo = ProcessInfo.FromProject(project);
            GdbProcessInstance = new GdbProcess(processInfo, debugFilePath);
            DebuggerProcessInstance = new DebuggerProcess(processInfo, scriptGroupType, scriptHash, txFilePath, ioType, ioIndex, binaryForDebugger);
            DebuggerProcessInstance.Start();
            GdbProcessInstance.Start();
        }

        public static void Stop()
        {
            GdbProcessInstance?.Stop();
            DebuggerProcessInstance?.Stop();
        }
    }
}
