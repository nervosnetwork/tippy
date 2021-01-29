using System;
using Ckb.Types;

namespace Tippy.Pages.TxPool
{
    public class IndexModel : PageModelBase
    {
        public TxPoolInfo? TxPoolInfo { get; set; }
        public RawTxPool? RawTxPool { get; set; }

        public IndexModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public void OnGet()
        {
            var rpc = Rpc();
            TxPoolInfo = rpc.TxPoolInfo();
            RawTxPool = rpc.GetRawTxPool();
        }
    }
}
