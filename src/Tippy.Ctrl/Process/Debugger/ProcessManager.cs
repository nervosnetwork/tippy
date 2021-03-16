namespace Tippy.Ctrl.Process.Debugger
{
    public class ProcessManager
    {
        private static GdbProcess? GdbProcessInstance;
        private static DebuggerProcess? DebuggerProcessInstance;

        public static void Start(string scriptGroupType, string scriptHash, string txFilePath, string debugFilePath, string ioType, int ioIndex)
        {
            Stop();
            // TODO: Replace with ActiveProject or remove it.
            ProcessInfo processInfo = new ProcessInfo(1, Tippy.Core.Models.Project.ChainType.Dev, 8114, 8115, 8116, "");
            GdbProcessInstance = new GdbProcess(processInfo, debugFilePath);
            DebuggerProcessInstance = new DebuggerProcess(processInfo, scriptGroupType, scriptHash, txFilePath, ioType, ioIndex);
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
