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
            var activeProject = await _context.Projects.FirstOrDefaultAsync(p => p.IsActive);
            activeProject ??= await _context.Projects.FirstOrDefaultAsync(p => true); // Fall back to none active project
            context.HttpContext.Items["ActiveProject"] = activeProject;
            await next();
        }
    }
}
