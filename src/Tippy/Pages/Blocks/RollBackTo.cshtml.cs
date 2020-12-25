using System.Threading.Tasks;
using Ckb.Rpc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.Blocks
{
    public class RollBackToModel : PageModelBase
    {
        public RollBackToModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnPostAsync(string? hash)
        {
            if (hash == null)
            {
                return NotFound();
            }

            if (ActiveProject != null)
            {
                Client rpc = new($"http://localhost:{ActiveProject.NodeRpcPort}");
                rpc.Truncate(hash);
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
