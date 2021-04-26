using System;
using System.Collections.Generic;
using System.Linq;
using Ckb.Types;
using Tippy.Core.Models;

namespace Tippy.Pages.TxPool
{
    public class IndexModel : PageModelBase
    {
        public TxPoolInfo? TxPoolInfo { get; set; }
        public RawTxPool? RawTxPool { get; set; }
        public HashSet<string> ProposeDenyList = default!;
        public HashSet<string> CommitDenyList = default!;

        public IndexModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public void OnGet()
        {
            DbContext.Entry(ActiveProject!)
                .Collection(p => p.DeniedTransactions)
                .Load();
            var deniedTransactions = ActiveProject!.DeniedTransactions;
            ProposeDenyList = new HashSet<string>(deniedTransactions
                .Where(d => d.DenyType == DeniedTransaction.Type.Propose)
                .Select(d => d.TxHash));
            CommitDenyList = new HashSet<string>(deniedTransactions.
                Where(d => d.DenyType == DeniedTransaction.Type.Commit)
                .Select(d => d.TxHash));

            var rpc = Rpc();
            TxPoolInfo = rpc.TxPoolInfo();
            RawTxPool = rpc.GetRawTxPool();
        }
    }
}
