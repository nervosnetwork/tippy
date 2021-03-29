using System.Collections.Generic;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Projects
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.TippyDbContext context) : base(context)
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
