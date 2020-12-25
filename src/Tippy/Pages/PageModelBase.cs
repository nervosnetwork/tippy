using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;

namespace Tippy.Pages
{
    /// <summary>
    /// Base page model for pages. Get pages should have page model inherited from this
    /// to have common properties used by layout. Post pages may decide if they should
    /// use inherited page model or not.
    /// </summary>
    public class PageModelBase : PageModel
    {
        protected readonly Tippy.Core.Data.DbContext DbContext;

        public IList<Project> Projects { get; set; } = new List<Project>();
        public Project? ActiveProject { get; set; }

        protected PageModelBase(Tippy.Core.Data.DbContext context)
        {
            DbContext = context;
        }

        public override async Task OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            Projects = await DbContext.Projects.ToListAsync();
            ActiveProject = await DbContext.Projects.FirstOrDefaultAsync(p => p.IsActive);
            if (ActiveProject == null && Projects.Count > 0)
            {
                ActiveProject = Projects[0]; // Fall back to none active project
            }

            await next();
        }

        public override Task OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
