using System;
using Ckb.Rpc;
using Ckb.Types;

namespace Tippy.Pages.Cells
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public Script LockScript { get; set; } = default!;
        public Script? TypeScript { get; set; }
        public string Data { get; set; } = "";

        public void OnGet(string id)
        {
            Client client = Rpc();
            var (txHash, index) = ParseId(id);
            var tx = GetTransaction(client, txHash);

            LockScript = tx.Outputs[index].Lock;
            TypeScript = tx.Outputs[index].Type;
            Data = tx.OutputsData[index];
        }

        private static Transaction GetTransaction(Client client, string txHash)
        {
            var txWithStatus = client.GetTransaction(txHash);
            if (txWithStatus == null)
            {
                throw new Exception("No transaction found!");
            }
            return txWithStatus.Transaction;
        }

        private static (string TxHash, int Index) ParseId(string id)
        {
            string[] info = id.Split(':');
            if (info.Length != 2)
            {
                throw new Exception($"Id: {id} format error!");
            }
            return (info[0], Int32.Parse(info[1]));
        }
    }
}
