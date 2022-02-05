using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;

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
       
            process.StartInfo.FileName = BinaryFullPath(WorkPathManage.CkbForPaltform(ckbenum.ckb));
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
          
           process.StartInfo.FileName = BinaryFullPath(WorkPathManage.CkbForPaltform(ckbenum.ckb));
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = BuildArguments();

          

            process.Start();
           
             StreamWriter writer = process.StandardInput;
             writer.Write(ChainSpec());
             writer.Close();

            process.WaitForExit();
            CopyTheConstractFiles(ProcessInfo.Contracts);
        }

        public void Reset()
        {
            try
            {
                var filepath = Path.Combine(WorkingDirectory(), "indexer-data");
                if (File.Exists(filepath))
                Directory.Delete(filepath, true);
              
            }
            catch (Exception e)
            {
                Console.WriteLine($"Failed to reset project data, {e.Message}");
            }
            try
            {
                var filepath = Path.Combine(WorkingDirectory(), "data");
                if (File.Exists(filepath))
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
                        return $"args = \"{ProcessInfo.LockArg}\"";
                    }
                    else if (line.StartsWith("listen_addresses ="))
                    {
                        return $"listen_addresses = [\"/ip4/0.0.0.0/tcp/{ProcessInfo.NodeNetworkPort}\"]";
                    }
                    else if (line.StartsWith("listen_address ="))
                    {
                        return $"listen_address = \"127.0.0.1:{ProcessInfo.NodeRpcPort}\"";
                    }
                    else if (line.StartsWith("modules ="))
                    {
                        return $"modules = [\"Net\", \"Pool\", \"Miner\", \"Chain\", \"Stats\", \"Subscription\", \"Experiment\", \"IntegrationTest\", \"Debug\"]";
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
       static string chainspectemplatefile => Path.Combine(new[] { AppContext.BaseDirectory, "BinDeps", "ChainSpecTemplate.txt" });

        string BuildArguments()
        {
            return ProcessInfo.Chain switch
            {
                Core.Models.Project.ChainType.Testnet => $"init --chain testnet --ba-arg {ProcessInfo.LockArg}",
                Core.Models.Project.ChainType.Mainnet => $"init --chain mainnet --ba-arg {ProcessInfo.LockArg}",
                _ => $"init --chain dev --ba-arg {ProcessInfo.LockArg} --import-spec -",
            };
        }

        string ChainSpec()
        {
            var spec = DevChainSpecTemplate;
            spec = spec.Replace("[GENESIS_CELL_MESSAGE]", "ckb_dev_" + ProcessInfo.ID);
            spec = spec.Replace("0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7", ProcessInfo.LockArg);
            var initcapacity = ModilfySpecToml.InitCapacity(ProcessInfo.InitCapacity);
            var cstomesConstracts = ModilfySpecToml.InitCstomesConstracts(ProcessInfo.Contracts);
            if (!string.IsNullOrEmpty(initcapacity))
            {
                spec = spec.Replace("{{Capacity}}", initcapacity);
            }
            if (!string.IsNullOrEmpty(initcapacity))
            {
                spec = spec.Replace("{{Customes_System_Cells}}", cstomesConstracts);
            }
            else
            {
                spec = spec.Replace("{{Customes_System_Cells}}", string.Empty);
            }
            if (!String.IsNullOrEmpty(ProcessInfo.ExtraToml))
            {
                spec += "\n" + ProcessInfo.ExtraToml + "\n";
            }
            var bytes = Encoding.UTF8.GetBytes(spec);
            return Convert.ToBase64String(bytes);
        }

        private static string DevChainSpecTemplate
        {
            get
            {
                //var resourceName = "Tippy.Ctrl.ChainSpecTemplate.txt";
                //using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName)!;
                //using StreamReader reader = new StreamReader(stream);
                //return reader.ReadToEnd();

                if (File.Exists(chainspectemplatefile))
                {
                    return System.IO.File.ReadAllText(chainspectemplatefile);
                }
                else
                {
                    return "";
                }
                 
            }
        }
    }
}
