using System;
using Tippy.Ctrl;

namespace Tippy.Pages.Miners
{
    public class AdvancedModel : PageModelBase
    {
        public bool IsMinerRunning = default;

        public AdvancedModel(Tippy.Core.Data.DbContext context) : base(context)
        {
        }

        public void OnGet()
        {
            IsMinerRunning = ProcessManager.IsMinerRunning(ActiveProject!);
        }
    }
}
