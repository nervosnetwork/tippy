using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;

namespace Tippy.Pages.Home
{
    public class IndexModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public IndexModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public IList<Project> Projects { get; set; }
        public Project ActiveProject { get; set; }

        [TempData]
        public string Message { get; set; }

        public bool IsNodeRunning { get; set; }

        public async Task OnGetAsync()
        {
            Projects = await _context.Projects.ToListAsync();
            ActiveProject = Projects.FirstOrDefault(p => p.IsActive) ?? Projects[0];
            IsNodeRunning = false;
        }
    }
}
