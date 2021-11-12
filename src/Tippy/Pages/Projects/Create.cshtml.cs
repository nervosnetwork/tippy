using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Ckb.Address;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Shared.MyConstracts;
namespace Tippy.Pages.Projects
{
    public class CreateModel : PageModelBase
    {
        public CreateModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        [BindProperty]

        [Display(Name = "Constracts")]
        public List<Domain_ConstractFile>  ConstractFiles{ get; set; }

        public void OnGet()
        {

            ConstractFiles = WorkPathManage.GetListOfConstractFiles().Select(item => new Domain_ConstractFile { filename= item.Name, filepath=item.FullName}).ToList() ;
            var calculatingFromUsed = Projects.Count > 0;
            var rpcPorts = Projects.Select(p => p.NodeRpcPort);
            var networkPorts = Projects.Select(p => p.NodeNetworkPort);
            var indexerPorts = Projects.Select(p => p.IndexerRpcPort);

            Project = new Project
            {
                Name = $"CKB Chain",
                Chain = Project.ChainType.Dev,
                NodeRpcPort = calculatingFromUsed ? rpcPorts.Max() + 3 : 8114,
                NodeNetworkPort = calculatingFromUsed ? networkPorts.Max() + 3 : 8115,
                IndexerRpcPort = calculatingFromUsed ? indexerPorts.Max() + 3 : 8116,
                LockArg = "0xc8328aabcd9b9e8e64fbc566c4385c3bdeb219d7"
            };
        }

        [BindProperty]
        public Project Project { get; set; } = default!;

        // To protect from overposting attacks, see https://aka.ms/RazorPagesCRUD
        public async Task<IActionResult> OnPostAsync()
        {
            if (Project.LockArg.StartsWith("ckb") || Project.LockArg.StartsWith("ckt"))
            {
                Project.LockArg = Address.ParseAddress(Project.LockArg, Project.LockArg.Substring(0, 3)).Args;
            }
           
           
            Project.Tokens = new List<Token>();
            Project.RecordedTransactions = new List<RecordedTransaction>();
            Project.IsActive = !Projects.Any(p => p.IsActive);
            DbContext.Projects.Add(Project);
            await DbContext.SaveChangesAsync();
            foreach (var item in ConstractFiles)
            {
                if (item.selectforintockb)
                {
                    DbContext.Contracts.Add(new Contracts
                    {
                        ProjectId = Project.Id,
                        createtime = DateTime.Now,
                        filename = item.filename,
                        filepath = item.filepath
                    });
                }

            }
            await DbContext.SaveChangesAsync();

            
          
            return RedirectToPage("./Index");
        }
    }
}
