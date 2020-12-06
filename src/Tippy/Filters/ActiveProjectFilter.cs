using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace Tippy.Filters
{
    public class ActiveProjectFilter : Attribute, IAsyncResourceFilter
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public ActiveProjectFilter(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        async Task IAsyncResourceFilter.OnResourceExecutionAsync(ResourceExecutingContext context, ResourceExecutionDelegate next)
        {
            var projects = await _context.Projects.ToListAsync();
            var activeProject = await _context.Projects.FirstOrDefaultAsync(p => p.IsActive);
            if (activeProject == null && projects.Count > 0)
            {
                activeProject = projects[0]; // Fall back to none active project
            }
            context.HttpContext.Items["Projects"] = projects;
            context.HttpContext.Items["ActiveProject"] = activeProject;
            await next();
        }
    }
}
