using System;
using System.Diagnostics;

namespace Tippy.Ctrl.Process
{
    class IndexerProcess : CommandProcess
    {
        protected override void Configure()
        {
            process = new System.Diagnostics.Process();
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.FileName = BinaryFullPath("ckb-indexer");
            process.StartInfo.WorkingDirectory = WorkingDirectory();
            process.StartInfo.Arguments = "-s indexer-data";
            // TODO: pass in listen_uri and ckb_uri
        }
    }
}
