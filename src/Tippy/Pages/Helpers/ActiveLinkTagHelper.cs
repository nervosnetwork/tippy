using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.Runtime.TagHelpers;
using Microsoft.AspNetCore.Razor.TagHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Tippy.Pages.Helpers
{
    // You may need to install the Microsoft.AspNetCore.Razor.Runtime package into your project
    [HtmlTargetElement("tag-name")]
    public class ActiveLinkTagHelper : TagHelper
    {
        public override void Process(TagHelperContext context, TagHelperOutput output)
        {

        }
    }

    public static class HtmlHelperExtensions
    {
        // TODO: migrate this to a TagHelper to support <a asp-page.../>
        public static string ActiveClass(this IHtmlHelper htmlHelper, string route)
        {
            var pageRoute = htmlHelper.ViewContext.RouteData.Values["page"].ToString();
            return route == pageRoute ? "is-active" : "";
        }
    }
}
