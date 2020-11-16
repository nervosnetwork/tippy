using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tippy.Pages
{
    public class HomeModel : PageModel
    {
        [TempData]
        public string Message { get; set; }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Ctrl.ProcessManager.Restart();

            Message = "Node has been restarted.";
            return RedirectToPage();
        }
    }
}
