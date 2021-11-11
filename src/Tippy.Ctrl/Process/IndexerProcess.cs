using System;

namespace Tippy.Ctrl.Process
{
    internal class IndexerProcess : CommandProcess
    {
        internal IndexerProcess(ProcessInfo info) : base(info) { }

        protected override void Configure()
        {
            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            //process.StartInfo.FileName = BinaryFullPath("ckb-indexer.exe");
            process.StartInfo.FileName = BinaryFullPath(WorkPathManage.CkbForPaltform(ckbenum.ckbindexer));
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = $"-s indexer-data -c http://127.0.0.1:{ProcessInfo.NodeRpcPort} -l 127.0.0.1:{ProcessInfo.IndexerRpcPort}";
        }
    }
}
