using System;
using System.IO;

namespace Tippy.Ctrl.Process.Debugger
{
    internal class DebuggerProcess : CommandProcess
    {
        private string ScriptHash;
        private string ScriptGroupType;
        private string TxFilePath;
        private string IoType;
        private int IoIndex;

        public DebuggerProcess(ProcessInfo info, string scriptGroupType, string scriptHash, string txFilePath, string ioType, int ioIndex) : base(info)
        {
            ScriptHash = scriptHash;
            ScriptGroupType = scriptGroupType;
            TxFilePath = txFilePath;
            IoType = ioType;
            IoIndex = ioIndex;
        }

        protected override void Configure()
        {
            string? workingDirectory = Path.GetDirectoryName(TxFilePath);
            if (workingDirectory == null)
            {
                throw new Exception("No file path found!");
            }
            string arguments = $"--port 7682 ckb-debugger -l 0.0.0.0:2000 -g {ScriptGroupType} -h {ScriptHash} -t {TxFilePath} -e {IoType} -i {IoIndex}";
            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "ttyd";
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.Arguments = arguments;
        }
    }
}
