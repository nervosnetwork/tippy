using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace Tippy.Pages.Config
{
    public class IndexModel : PageModel
    {
        [Required]
        [BindProperty]
        [Display(Name = "Open browser on launch")]
        public bool OpenBrowserOnLaunch { get; set; } = Core.Settings.GetSettings().AppSettings.OpenBrowserOnLaunch;

        [TempData]
        public string Message { get; set; } = "";

        public void OnGet()
        {
        }

        public IActionResult OnPost()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var settings = Core.Settings.GetSettings();
            settings.AppSettings.OpenBrowserOnLaunch = OpenBrowserOnLaunch;
            settings.Save();

            Message = "Settings saved";
            return RedirectToPage();
        }
    }
}
