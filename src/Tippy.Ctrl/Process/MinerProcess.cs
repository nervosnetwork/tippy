using System;
using System.IO;
using System.Linq;

namespace Tippy.Ctrl.Process
{
    internal class MinerProcess : CommandProcess
    {
        internal MinerProcess(ProcessInfo info) : base(info) { }

        protected override void Configure()
        {
            UpdateConfiguration();

            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
           
            process.StartInfo.FileName = BinaryFullPath(WorkPathManage.CkbForPaltform(ckbenum.ckb));
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = "miner";
        }

        void UpdateConfiguration()
        {
            try
            {
                var tomlContent = File.ReadLines(TomlFile);
                var updatedContent = tomlContent.Select((line) =>
                {
                    if (line.StartsWith("rpc_url = "))
                    {
                        return $"rpc_url = \"http://127.0.0.1:{ProcessInfo.NodeRpcPort}\"";
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

        string TomlFile => Path.Combine(WorkingDirectory(), "ckb-miner.toml");
    }
}
