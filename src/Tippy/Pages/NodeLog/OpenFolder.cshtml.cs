using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;
using Tippy.Util;

namespace Tippy.Pages.NodeLog
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class OpenFolderModel : PageModel
    {
        public async Task<IActionResult> OnPostAsync()
        {
            if (HttpContext.Items["ActiveProject"] is Project activeProject && ProcessManager.IsRunning(activeProject))
            {
                await Task.Run(() =>
                {
                    FolderOpener.Open(ProcessManager.GetLogFolder(activeProject));
                });
            }

            return RedirectToPage("./Index");
        }
    }
}
