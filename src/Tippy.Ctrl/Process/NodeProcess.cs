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
            // TODO: read from user config for Block Assembler
            var lockArg = "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7";
            return lockArg;
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
                var assembly = Assembly.GetExecutingAssembly();
                var resourceName = "Tippy.Ctrl.ChainSpecTemplate.txt";
                using Stream stream = assembly.GetManifestResourceStream(resourceName);
                using StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}
