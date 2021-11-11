using System;
using System.Collections.Generic;

namespace Tippy.Ctrl.Process
{
    internal class BinariesInfo
    {
        internal string Info { get; private set; } = "";
        internal bool HasDebuggerDeps { get; private set; } = false;
        readonly List<string> binaries = new() { WorkPathManage.CkbForPaltform(ckbenum.ckb), WorkPathManage.CkbForPaltform(ckbenum.ckbindexer), WorkPathManage.CkbForPaltform(ckbenum.ckbdebugger)/*, "ckb-cli"*/ };
        //readonly List<string> binaries = new() { "ckb.exe", "ckb-indexer.exe", "ckb-debugger.exe"/*, "ckb-cli"*/ };
        internal void Refresh()
        {
            foreach (var binary in binaries)
            {
               ///ckb - debugger not supported on Windows yet.
                if (OperatingSystem.IsWindows() && binary == WorkPathManage.CkbForPaltform(ckbenum.ckbdebugger))
                {
                    continue;
                }

                System.Diagnostics.Process process = new();
                process.StartInfo.UseShellExecute = false;
                process.StartInfo.FileName = CommandProcess.BinaryFullPath(binary);
                process.StartInfo.Arguments = "--version";
                process.StartInfo.WorkingDirectory = Core.Environment.GetAppDataFolder();

                process.StartInfo.RedirectStandardOutput = true;
                process.OutputDataReceived += (sender, e) =>
                {
                    if (!String.IsNullOrEmpty(e.Data))
                    {
                        Info += e.Data + "\n";
                    }
                };

                process.Start();
                process.BeginOutputReadLine();
                process.WaitForExit();
            }

            if (!OperatingSystem.IsWindows())
            {
                RefreshDebuggerDeps();
            }
        
        }

        void RefreshDebuggerDeps()
        {
            HasDebuggerDeps = true;
            foreach (var dep in new List<string>() { "gdb", "ttyd" })
            {
                try
                {
                    System.Diagnostics.Process process = new();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = dep;
                    process.StartInfo.Arguments = "--version";
                    process.StartInfo.WorkingDirectory = Core.Environment.GetAppDataFolder();
                    process.StartInfo.RedirectStandardOutput = true;
                    process.OutputDataReceived += (sender, e) =>
                    {
                        // Do not show gdb/ttyd
                    };
                    process.Start();
                    process.BeginOutputReadLine();
                    process.WaitForExit();
                }
                catch
                {
                    HasDebuggerDeps = false;
                }
            }
        }
    }
}
