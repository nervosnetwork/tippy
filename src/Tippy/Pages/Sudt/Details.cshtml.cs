using Ckb.Types;

namespace Tippy.Pages.Sudt
{
    public class DetailsModel : PageModelBase
    {
        public DetailsModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public Script SudtScript { get; set; } = default!;

        public void OnGet(string sudtScriptArgs)
        {
            SudtScript = new Script
            {
                CodeHash = Helpers.TransactionHelper.SudtCodeHash,
                HashType = Helpers.TransactionHelper.SudtHashType,
                Args = sudtScriptArgs,
            };
        }
    }
}
