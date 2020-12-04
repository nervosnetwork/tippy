using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;

namespace Tippy.Pages.NodeLog
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class IndexModel : PageModel
    {
        public Project? ActiveProject { get; set; }

        public void OnGet()
        {
            if (HttpContext.Items["ActiveProject"] is Project activeProject && ProcessManager.IsRunning(activeProject))
            {
                ActiveProject = activeProject;
            }
        }
    }
}