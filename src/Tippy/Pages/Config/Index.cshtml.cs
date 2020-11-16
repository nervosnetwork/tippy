using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tippy.Pages.Config
{
    public class IndexModel : PageModel
    {
        [Required]
        [BindProperty]
        public string LockArg { get; set; } = Core.Settings.GetSettings().BlockAssembler.LockArg;

        [TempData]
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var settings = Core.Settings.GetSettings();
            settings.BlockAssembler.LockArg = LockArg;
            settings.Save();
            Ctrl.ProcessManager.UpdateConfiguration();

            Message = "Lock arg was updated.";
            return RedirectToPage();
        }
    }
}
