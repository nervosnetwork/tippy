using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tippy.Core.Models
{
    public class Contracts
    {

        public int id { get; set; }

         
        public int ProjectId { get; set; }

        public Project Project { get; set; } = default!;

        public string  filename  { get; set; }

        public string  filepath { get; set; }

        public DateTime createtime { get; set; }



    }
}
