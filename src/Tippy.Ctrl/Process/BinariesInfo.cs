using System;
using System.Collections.Generic;

namespace Tippy.Ctrl.Process
{
    internal class BinariesInfo
    {
        internal string Info { get; private set; } = "";
        internal bool HasDebuggerDeps { get; private set; } = false;

        readonly List<string> binaries = new() { "ckb", "ckb-indexer", "ckb-debugger"/*, "ckb-cli"*/ };

        internal void Refresh()
        {
            foreach (var binary in binaries)
            {
                // ckb-debugger not supported on Windows yet.
                if (OperatingSystem.IsWindows() && binary == "ckb-debugger")
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
            // BUGBUG: .Net won't find commands on M1 Mac which has homebrew location at `/opt/homebrew/bin`.
            foreach (var dep in new List<string>() { "gdb", "ttyd" })
            {
                try
                {
                    System.Diagnostics.Process process = new();
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.FileName = dep;
                    process.StartInfo.Arguments = "--version";
                    process.StartInfo.WorkingDirectory = Core.Environment.GetAppDataFolder();
                    process.Start();
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
