using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
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
            var activeProject = projects.FirstOrDefault(p => p.IsActive) ?? projects[0];
            context.HttpContext.Items["ActiveProject"] = activeProject;
            await next();
        }
    }
}
