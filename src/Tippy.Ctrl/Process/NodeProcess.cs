using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Linq;

namespace Tippy.Ctrl.Process
{
    internal class NodeProcess : CommandProcess
    {
        internal NodeProcess(ProcessInfo info) : base(info) { }

        protected override void Configure()
        {
            InitalizeCkbIfNecessary();
            UpdateConfiguration();

            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = BinaryFullPath("ckb");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = "run";
        }

        void InitalizeCkbIfNecessary()
        {
            if (File.Exists(TomlFile))
            {
                return;
            }

            using System.Diagnostics.Process process = new();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardInput = true;
            process.StartInfo.FileName = BinaryFullPath("ckb");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = BuildArguments(); // TODO: only load spec template for devnet
            process.Start();

            StreamWriter writer = process.StandardInput;
            writer.Write(ChainSpec());
            writer.Close();

            process.WaitForExit();
        }

        public void Reset()
        {
            try
            {
                Directory.Delete(Path.Combine(WorkingDirectory(), "indexer-data"), true);
                Directory.Delete(Path.Combine(WorkingDirectory(), "data"), true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to reset project data, {e.Message}");
            }
        }

        void UpdateConfiguration()
        {
            try
            {
                var tomlContent = File.ReadLines(TomlFile);
                var updatedContent = tomlContent.Select((line) =>
                {
                    if (line.StartsWith("args = "))
                    {
                        return $"args = \"{LockArg}\"";
                    }
                    else if (line.StartsWith("listen_addresses ="))
                    {
                        return $"listen_addresses = [\"/ip4/0.0.0.0/tcp/{ProcessInfo.NodeNetworkPort}\"]";
                    }
                    else if (line.StartsWith("listen_address ="))
                    {
                        return $"listen_address = \"127.0.0.1:{ProcessInfo.NodeRpcPort}\"";
                    }
                    else
                    {
                        return line;
                    }
                });
                File.WriteAllLines(TomlFile, updatedContent.ToArray());
            }
            catch
            { }
        }

        string TomlFile => Path.Combine(WorkingDirectory(), "ckb.toml");

        string BuildArguments() => $"init --chain dev --ba-arg {LockArg} --import-spec -";

        string LockArg => Core.Settings.GetSettings().BlockAssembler.LockArg;

        string ChainSpec()
        {
            var spec = DevChainSpecTemplate;
            spec = spec.Replace(
                "[GENESIS_CELL_MESSAGE]",
                "ckb_dev_" + DateTime.Now.ToString("yyyyMMddHHmmss",
                CultureInfo.InvariantCulture));
            var bytes = Encoding.UTF8.GetBytes(spec);
            return Convert.ToBase64String(bytes);
        }

        private static string DevChainSpecTemplate
        {
            get
            {
                var resourceName = "Tippy.Ctrl.ChainSpecTemplate.txt";
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
                using StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }
}
