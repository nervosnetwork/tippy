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

        public bool IsNodeRunning { get; set; }

        public void OnGet()
        {
            IsNodeRunning = Ctrl.ProcessManager.IsRunning;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            Message = "";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostRestartAsync()
        {
            Ctrl.ProcessManager.Restart();

            Message = "Node has been restarted.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostStartAsync()
        {
            Ctrl.ProcessManager.Start();

            Message = "Node has been started.";
            return RedirectToPage();
        }

        public async Task<IActionResult> OnPostResetAsync()
        {
            Ctrl.ProcessManager.ResetData();

            Message = "Node data has been reset. Node has been restarted.";
            return RedirectToPage();
        }
    }
}
