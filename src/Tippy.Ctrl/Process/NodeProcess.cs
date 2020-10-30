using System;
using System.IO;

namespace Tippy.Ctrl.Process
{
    class NodeProcess : CommandProcess
    {
        protected override void Configure()
        {
            InitalizeCkbIfNecessary();

            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = BinaryFullPath("ckb");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = "run";

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

        void InitalizeCkbIfNecessary()
        {
            if (File.Exists(Path.Combine(WorkingDirectory(), "ckb.toml")))
            {
                return;
            }

            // TODO: Block Assembler
            var process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = BinaryFullPath("ckb");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = "init --chain dev";
            process.Start();
            process.WaitForExit();
        }
    }
}
