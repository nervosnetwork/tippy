using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Home
{
    public class QuickStartModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public QuickStartModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Project project = new()
            {
                Name = "CKB devchain",
                Chain = Project.ChainType.Dev,
                NodeRpcPort = "8114",
                NodeNetworkPort = "8115",
                IndexerRpcPort = "8116",
                LockArg = "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7",
                IsActive = true
            };

            _context.Projects.Add(project);
            await _context.SaveChangesAsync();
            ProcessManager.Start(project);

            return RedirectToPage("./Index");
        }
    }
}
