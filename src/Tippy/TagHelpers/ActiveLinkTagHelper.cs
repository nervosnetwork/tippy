using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace Tippy.TagHelpers
{
    [HtmlTargetElement("a")]
    public class ActiveLinkTagHelper : AnchorTagHelper
    {
        public ActiveLinkTagHelper(IHtmlGenerator htmlGenerator) : base(htmlGenerator)
        {
        }

        public override void Process(TagHelperContext context, TagHelperOutput output)
        {
            base.Process(context, output);

            var currentRoute = ViewContext.RouteData.Values["page"].ToString();
            var tagRoute = context.AllAttributes["asp-page"]?.Value.ToString();
            var className = currentRoute == tagRoute ? "is-active" : "";

            var tag = new TagBuilder("a");
            tag.Attributes.Add("class", className);
            output.MergeAttributes(tag);
        }
    }
}
