using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Filters;

namespace Tippy.Pages.Projects
{
    [ServiceFilter(typeof(ActiveProjectFilter))]
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public Dictionary<Project, bool> RunningFlags { get; set; } = default!;

        public void OnGet()
        {
            RunningFlags = new Dictionary<Project, bool>();
            foreach (var p in Projects)
            {
                RunningFlags.Add(p, ProcessManager.IsRunning(p));
            }
        }
    }
}
