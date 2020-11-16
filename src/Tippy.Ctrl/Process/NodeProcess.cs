using System;
using System.IO;
using System.Text;
using System.Reflection;
using System.Globalization;
using System.Linq;

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
            if (File.Exists(TomlFile))
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

        public static void UpdateConfiguration()
        {
            try
            {
                var tomlContent = File.ReadLines(TomlFile);
                var updatedContent = tomlContent.Select((line) =>
                {
                    if (line.StartsWith("args = ", System.StringComparison.Ordinal))
                    {
                        return $"args = \"{LockArg}\"";

                    } else
                    {
                        return line;
                    }
                });
                File.WriteAllLines(TomlFile, updatedContent.ToArray());
            } catch
            {}
        }

        static string TomlFile
        {
            get
            {
                return Path.Combine(WorkingDirectory(), "ckb.toml");
            }
        }

        static string BuildArguments()
        {
            return $"init --chain dev --ba-arg {LockArg} --import-spec -";
        }

        static string LockArg
        {
            get
            {
                return Core.Settings.GetSettings().BlockAssembler.LockArg;
            }
        }

        static string ChainSpec()
        {
            var spec = ChainSpecTemplate;
            spec = spec.Replace("[GENESIS_CELL_MESSAGE]", "ckb_dev_" + DateTime.Now.ToString("yyyyMMddHHmmss", CultureInfo.InvariantCulture));
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
