using System;
using System.IO;

namespace Tippy.Ctrl.Process.Debugger
{
    internal class DebuggerProcess : CommandProcess
    {
        private readonly string ScriptHash;
        private readonly string ScriptGroupType;
        private readonly string TxFilePath;
        private readonly string IoType;
        private readonly int IoIndex;
        private readonly string? BinaryPath;

        public DebuggerProcess(ProcessInfo info, string scriptGroupType, string scriptHash, string txFilePath, string ioType, int ioIndex, string? binaryPath = null) : base(info)
        {
            ScriptHash = scriptHash;
            ScriptGroupType = scriptGroupType;
            TxFilePath = txFilePath;
            IoType = ioType;
            IoIndex = ioIndex;
            BinaryPath = binaryPath;
        }

        protected override void Configure()
        {
            string? workingDirectory = Path.GetDirectoryName(TxFilePath);
            if (workingDirectory == null)
            {
                throw new Exception("No file path found!");
            }
            string debuggerBinaryPath = BinaryFullPath("ckb-debugger");
            string arguments = $"--port 7682 {debuggerBinaryPath} -l 0.0.0.0:2000 -g {ScriptGroupType} -h {ScriptHash} -t {TxFilePath} -e {IoType} -i {IoIndex}";
            if (BinaryPath != null)
            {
                arguments += $" -r {BinaryPath}";
            }
            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "ttyd";
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.Arguments = arguments;
        }
    }
}
