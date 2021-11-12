using System;
using System.IO;
using System.Reflection;

namespace Tippy.Ctrl.Process.Debugger
{
    internal class GdbProcess : CommandProcess
    {
        private readonly string DebugFilePath;

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
            CopyGdbInit();
            string arguments = $"gdb {DebugFilePath} -iex \"source {GdbInitFilePath}\" -ex \"target remote 127.0.0.1:2000\"";
            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = "ttyd";
            process.StartInfo.WorkingDirectory = workingDirectory;
            process.StartInfo.Arguments = arguments;
        }

        static string GdbInitFilePath => Path.Combine(Core.Environment.GetAppDataFolder(), ".gdbinit");

        private static void CopyGdbInit()
        {
            if (File.Exists(GdbInitFilePath))
            {
                return;
            }

            StreamWriter writer = new(GdbInitFilePath);
            writer.Write(GdbInitFile());
            writer.Close();
        }

        private static string GdbInitFile()
        {
            var resourceName = "Tippy.Ctrl.GdbDashboard.txt";
            using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
            using StreamReader reader = new(stream);
            return reader.ReadToEnd();
        }
    }
}
