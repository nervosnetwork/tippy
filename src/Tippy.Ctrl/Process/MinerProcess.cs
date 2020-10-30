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
            process.StartInfo.Arguments = "miner";
        }
    }
}
