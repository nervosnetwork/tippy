using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Tippy.Ctrl.Process
{
    abstract class CommandProcess
    {
        protected System.Diagnostics.Process process;

        abstract protected void Configure();

        public void Start() {
            if (!Directory.Exists(WorkingDirectory())) {
                try
                {
                    Directory.CreateDirectory(WorkingDirectory());
                }
                catch (Exception ex)
                {
                    Debug.Fail($"Failed to create folder {ex}");
                }
            }

            if (process == null)
            {
                Configure();
                HandleOutput();
            }
            process.Start();
            process.BeginErrorReadLine();
            process.BeginOutputReadLine();
        }

        public void Stop() { 
            if (process != null)
            {
                process.Kill();
                process.WaitForExit();
                process = null;
            }
        }

        protected void HandleOutput() {
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, e) =>
            {
                Console.WriteLine(e.Data);
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                Console.Error.WriteLine(e.Data);
            };
        }

        protected static string WorkingDirectory()
        {
            return Path.Combine(Core.Environment.GetAppDataFolder(), "devchain");
        }

        protected static string BinaryFullPath(string binary)
        {
            return Path.Combine(Path.Combine(BinDepsDirectory()), binary);
        }

        protected static string[] BinDepsDirectory()
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
