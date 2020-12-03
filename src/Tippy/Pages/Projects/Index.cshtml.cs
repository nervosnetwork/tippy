using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;

namespace Tippy.Pages.Projects
{
    public class IndexModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public IndexModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public IList<Project> Projects { get; set; } = new List<Project>();
        public Dictionary<Project, bool> RunningFlags { get; set; } = default!;

        public async Task OnGetAsync()
        {
            Projects = await _context.Projects.ToListAsync();
            RunningFlags = new Dictionary<Project, bool>();
            foreach (var p in Projects)
            {
                RunningFlags.Add(p, ProcessManager.IsRunning(p));
            }
        }
    }
}
