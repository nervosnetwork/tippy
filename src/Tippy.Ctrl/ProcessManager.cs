using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;

namespace Tippy.Ctrl
{
    public class ProcessManager
    {
        static Process ckbProcess;
        static Process indexerProcess;

        public static void Start()
        {
            StartCkb();
            StartIndexer();
        }

        public static void Stop()
        {
            StopIndexer();
            StopCkb();
        }

        public static void StartCkb()
        {
            InitalizeCkbIfNecessary();

            ckbProcess = new Process();
            ckbProcess.StartInfo.UseShellExecute = false;
            ckbProcess.StartInfo.FileName = BinaryFullPath("ckb");
            ckbProcess.StartInfo.WorkingDirectory = WorkingDirectory();
            ckbProcess.StartInfo.Arguments = "run";

            ckbProcess.StartInfo.RedirectStandardOutput = true;
            ckbProcess.StartInfo.RedirectStandardError = true;

            ckbProcess.OutputDataReceived += (sender, e) =>
            {
                Console.WriteLine(e.Data);
            };

            ckbProcess.Exited += (sender, e) =>
            {
                Console.WriteLine("Unable to start process");
            };

            ckbProcess.Start();
            ckbProcess.BeginOutputReadLine();
        }

        public static void StopCkb()
        {
            if (ckbProcess != null)
            {
                ckbProcess.Kill();
                ckbProcess.WaitForExit();
            }
        }

        static void InitalizeCkbIfNecessary()
        {
            if (File.Exists(Path.Combine(WorkingDirectory(), "ckb.toml")))
            {
                return;
            }
 
            var process = new Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = BinaryFullPath("ckb");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = "init";
            process.Start();
            process.WaitForExit();
        }

        public static void StartIndexer()
        {
            indexerProcess = new Process();
            indexerProcess.StartInfo.UseShellExecute = false;
            indexerProcess.StartInfo.FileName = BinaryFullPath("ckb-indexer");
            indexerProcess.StartInfo.WorkingDirectory = WorkingDirectory();
            indexerProcess.StartInfo.Arguments = "-s indexer-data";

            indexerProcess.StartInfo.RedirectStandardOutput = true;
            indexerProcess.StartInfo.RedirectStandardError = true;

            indexerProcess.OutputDataReceived += (sender, e) =>
            {
                Console.WriteLine(e.Data);
            };

            indexerProcess.Exited += (sender, e) =>
            {
                Console.WriteLine("Unable to start process");
            };

            indexerProcess.Start();
            indexerProcess.BeginOutputReadLine();
        }

        public static void StopIndexer()
        {
            if (indexerProcess != null)
            {
                indexerProcess.Kill();
                indexerProcess.WaitForExit();
            }
        }

        static string WorkingDirectory()
        {
            return Tippy.Core.Environment.GetAppDataFolder();
        }

        static string BinaryFullPath(string binary)
        {
            return Path.Combine(Path.Combine(BinDepsDirectory()), binary);
        }

        static string[] BinDepsDirectory()
        {
            var platformFolder = "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platformFolder = "mac";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platformFolder = "linux";
            }

            return new[] { AppContext.BaseDirectory, "BinDeps", platformFolder };
        }
    }
}
