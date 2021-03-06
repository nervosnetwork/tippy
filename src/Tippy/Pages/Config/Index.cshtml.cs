using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;

namespace Tippy.Pages.Config
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [Required]
        [BindProperty]
        [Display(Name = "Open browser on launch")]
        public bool OpenBrowserOnLaunch { get; set; } = Core.Settings.GetSettings().AppSettings.OpenBrowserOnLaunch;
        public string AppUrl = Core.Settings.GetSettings().AppUrl;

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
