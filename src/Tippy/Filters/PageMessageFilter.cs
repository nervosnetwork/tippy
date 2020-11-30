using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Tippy.Filters
{
    public class PageMessageFilter : IAsyncPageFilter
    {
        async Task IAsyncPageFilter.OnPageHandlerExecutionAsync(PageHandlerExecutingContext context, PageHandlerExecutionDelegate next)
        {
            ITempDataDictionary? tempData = null;
            PageContext? pageContext = null;

            if (context.HandlerInstance is Page page)
            {
                tempData = page.TempData;
                pageContext = page.PageContext;
            }
            else if (context.HandlerInstance is PageModel pageModel)
            {
                tempData = pageModel.TempData;
                pageContext = pageModel.PageContext;
            }

            if (pageContext != null && tempData != null)
            {
                pageContext.ViewData["ErrorMessage"] = tempData["ErrorMessage"];
            }

            await next.Invoke();
        }

        Task IAsyncPageFilter.OnPageHandlerSelectionAsync(PageHandlerSelectedContext context)
        {
            return Task.CompletedTask;
        }
    }
}
