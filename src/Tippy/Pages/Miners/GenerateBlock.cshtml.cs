using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Miners
{
    public class GenerateBlockModel : PageModelBase
    {
        public GenerateBlockModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [BindProperty]
        public Project? Project { get; set; }
        public void OnGet()
        {

        }

        public async Task<JsonResult> OnGetMinerMoreBlocks(int? id,int? count)
        {
            //if (id == null)
            //{
            //    return NotFound();
            //}

            Project = await DbContext.Projects
                .Include(p => p.DeniedTransactions)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();

            if (Project != null)
            {
                try
                {
                    for (int i = 0; i < count; i++)
                    {
                           ProcessManager.StartMiner(Project, ProcessManager.MinerMode.SingleBlock);
                    }
                 
                }
                catch (System.InvalidOperationException e)
                {
                    TempData["ErrorMessage"] = e.Message;
                }
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return new JsonResult("ok");
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Project = await DbContext.Projects
                .Include(p => p.DeniedTransactions)
                .Where(p => p.Id == id)
                .FirstOrDefaultAsync();



            if (Project != null)
            {
                try
                {
                   ProcessManager.StartMiner(Project, ProcessManager.MinerMode.SingleBlock);
                }
                catch (System.InvalidOperationException e)
                {
                    TempData["ErrorMessage"] = e.Message;
                }
            }

            var referer = Request.GetTypedHeaders().Referer.ToString().ToLower();
            return Redirect(referer);
        }
    }
}
