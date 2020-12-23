using System.Threading.Tasks;
using Ckb.Rpc;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.Core.Models;
using Tippy.Filters;

namespace Tippy.Pages.Blocks
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class RollBackToModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync(string? hash)
        {
            if (hash == null)
            {
                return NotFound();
            }

            if (HttpContext.Items["ActiveProject"] is Project activeProject)
            {
                Client rpc = new($"http://localhost:{activeProject.NodeRpcPort}");
                rpc.Truncate(hash);
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
