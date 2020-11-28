using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;

namespace Tippy.Pages.Projects
{
    public class CreateModel : PageModel
    {
        private readonly Tippy.Core.Data.DbContext _context;

        public CreateModel(Tippy.Core.Data.DbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnGetAsync()
        {
            var projects = await _context.Projects.ToListAsync();
            var calculatingFromUsed = projects.Count > 0;
            var rpcPorts = projects.Select(p => p.NodeRpcPort);
            var networkPorts = projects.Select(p => p.NodeNetworkPort);
            var indexerPorts = projects.Select(p => p.IndexerRpcPort);

            Project = new Project
            {
                Name = $"CKB Chain",
                Chain = Project.ChainType.Dev,
                NodeRpcPort = calculatingFromUsed ? rpcPorts.Max() + 3 : 8114,
                NodeNetworkPort = calculatingFromUsed ? networkPorts.Max() + 3 : 8115,
                IndexerRpcPort = calculatingFromUsed ? indexerPorts.Max() + 3 : 8116,
                LockArg = "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7"
            };

            return Page();
        }

        [BindProperty]
        public Project Project { get; set; }

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var projects = await _context.Projects.ToListAsync();
            Project.IsActive = !projects.Any(p => p.IsActive);
            _context.Projects.Add(Project);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
