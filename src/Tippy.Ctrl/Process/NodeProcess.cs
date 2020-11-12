using System;
using System.IO;
using System.Text;
using System.Reflection;

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
        }

        static void InitalizeCkbIfNecessary()
        {
            if (File.Exists(Path.Combine(WorkingDirectory(), "ckb.toml")))
            {
                return;
            }

            using System.Diagnostics.Process process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = BinaryFullPath("ckb");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = BuildArguments();
            process.Start();

            StreamWriter writer = process.StandardInput;
            writer.Write(ChainSpec());
            writer.Close();

            process.WaitForExit();
        }

        static string BuildArguments()
        {
            return $"init --chain dev --ba-arg {LockArg()} --import-spec -";
        }

        static string LockArg()
        {
            return Core.Settings.BlockAssembler.LockArg;
        }

        static string ChainSpec()
        {
            var spec = ChainSpecTemplate;
            spec = spec.Replace("[GENESIS_CELL_MESSAGE]", "ckb_dev_" + DateTime.Now.ToString("yyyyMMddHHmmss"));
            var bytes = Encoding.UTF8.GetBytes(spec);
            return Convert.ToBase64String(bytes);
        }

        private static string ChainSpecTemplate
        {
            get
            {
                var resourceName = "Tippy.Ctrl.ChainSpecTemplate.txt";
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                using StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}
