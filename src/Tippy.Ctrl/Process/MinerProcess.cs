using System;
using System.Diagnostics;

namespace Tippy.Ctrl.Process
{
    class MinerProcess : CommandProcess
    {
        protected override void Configure()
        {
            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = BinaryFullPath("ckb");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = "minder";

            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, e) =>
            {
                Console.WriteLine(e.Data);
            };

            process.Exited += (sender, e) =>
            {
                Console.WriteLine("Unable to start process");
            };
        }
    }
}
