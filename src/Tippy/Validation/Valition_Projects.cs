using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Tippy.Core.Models;

namespace Tippy.Validation
{
    public class Valition_Projects
    {
        protected readonly Tippy.Core.Data.TippyDbContext DbContext;
        public Valition_Projects(Tippy.Core.Data.TippyDbContext context)
        {
            DbContext = context;
        }
        public  Task<bool> CreateCustomValidation(Contracts contract, ModelStateDictionary modelState)
        {

            bool bl =  DbContext.Contracts.Where(p => p.filename == contract.filename).Count()<=0;

            return  Task.FromResult<bool>(bl);
        }
    }
}
