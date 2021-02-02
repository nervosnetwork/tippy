using System;
using System.Collections.Generic;

namespace Tippy.Ctrl.Process
{
    internal class BinariesInfo
    {
        internal string Info { get; private set; } = "";
        List<string> binaries = new List<string> { "ckb", "ckb-indexer"/*, "ckb-cli"*/ };

        internal void Refresh()
        {
            foreach (var binary in binaries)
            {
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
        }
    }
}
