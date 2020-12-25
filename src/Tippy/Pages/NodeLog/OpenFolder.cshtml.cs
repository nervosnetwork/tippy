using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Tippy.Ctrl;
using Tippy.Util;

namespace Tippy.Pages.NodeLog
{
    public class OpenFolderModel : PageModelBase
    {
        public OpenFolderModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ActiveProject != null && ProcessManager.IsRunning(ActiveProject))
            {
                await Task.Run(() =>
                {
                    FolderOpener.Open(ProcessManager.GetLogFolder(ActiveProject));
                });
            }

            return RedirectToPage("./Index");
        }
    }
}
