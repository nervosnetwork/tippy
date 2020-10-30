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
            if (process == null)
            {
                Configure();
            }
            process.Start();
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

        protected string WorkingDirectory()
        {
            return Tippy.Core.Environment.GetAppDataFolder();
        }

        protected string BinaryFullPath(string binary)
        {
            return Path.Combine(Path.Combine(BinDepsDirectory()), binary);
        }

        protected string[] BinDepsDirectory()
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
