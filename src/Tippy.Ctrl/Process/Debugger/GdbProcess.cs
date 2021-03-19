using System;
using System.IO;

namespace Tippy.Ctrl.Process.Debugger
{
    internal class GdbProcess : CommandProcess
    {
        private string DebugFilePath;

        public GdbProcess(ProcessInfo info, string debugFilePath) : base(info)
        {
            DebugFilePath = debugFilePath;
        }

        protected override void Configure()
        {
            string? workingDirectory = Path.GetDirectoryName(DebugFilePath);
            if (workingDirectory == null)
            {
                throw new Exception("No file path found!");
            }
            string gdbInitFile = BinaryFullPath(".gdbinit");
            string arguments = $"gdb {DebugFilePath} -iex \"source {gdbInitFile}\" -ex \"target remote 127.0.0.1:2000\"";
            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "ttyd";
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.Arguments = arguments;
        }
    }
}
