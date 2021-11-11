using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Tippy.Core.Models;
using Tippy.Ctrl;
using Tippy.Shared.MyConstracts;
using Tippy.Utils;

namespace Tippy.Pages.MyConstract
{
    public class IndexModel : PageModelBase
    {
        public IndexModel(Tippy.Core.Data.TippyDbContext context) : base(context)
        {
        }

        public Dictionary<Project, bool> RunningFlags { get; set; } = new Dictionary<Project, bool>();

        public List<Domain_ConstractFile> constractFiles = new List<Domain_ConstractFile>();

        [BindProperty]
        public FileUpload FileUpload { get; set; }

        
      
      
        
        public  IActionResult OnPost()
        {
           
            using (var fileStream = new FileStream(Path.Combine(WorkPathManage.ScriptDirectory(), FileUpload.UploadPrivateSchedule.FileName), FileMode.Create))
            {
                 FileUpload.UploadPrivateSchedule.CopyTo(fileStream);
            }

            return RedirectToPage("./Index");
        }

        /// <summary>
        /// get constract file list
        /// </summary>
        /// <returns></returns>
        public void OnGet()
        {
            Domain_ConstractFile constracts = new Domain_ConstractFile();

            constractFiles= constracts.GetConstractFiles();

        }
        /// <summary>
        /// get constract file list
        /// </summary>
        /// <returns></returns>
        
        public Task<IActionResult> OnPostDelete(string filepath)
        {
            if (System.IO.File.Exists(filepath))
                {
                 System.IO.File.Delete(filepath);
            }
            return Task.FromResult< IActionResult>( RedirectToPage("./Index"));
        }

    }
}
