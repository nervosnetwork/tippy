using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;

namespace Tippy.Pages.NodeLog
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

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
