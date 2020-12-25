using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Tippy.Filters;

namespace Tippy.Pages.Config
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.DbContext context) :base(context)
        {
        }

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
