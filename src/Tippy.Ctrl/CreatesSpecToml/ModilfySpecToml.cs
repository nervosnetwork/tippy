using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tippy.Core.Models;

namespace Tippy.Ctrl
{
   public  class ModilfySpecToml
    {
        /// <summary>
        /// Init Capacity
        /// </summary>
        /// <param name="initCapacity"></param>
        /// <returns></returns>
        public static string InitCapacity(UInt64 initCapacity)
        {
            string ptr = string.Empty;
            if (initCapacity > 0)
            {

                NumberFormatInfo nfi = new CultureInfo("en-US", false).NumberFormat;
                nfi.NumberGroupSeparator = "_";
                 ptr = initCapacity.ToString("N", nfi);
                if (ptr.Contains("."))
                {
                    ptr = ptr.Substring(0, ptr.IndexOf('.')); ;
                }
               
            }
            return ptr;
        }

        /// <summary>
        /// InitCstomesConstracts
        /// </summary>
        /// <param name="cons"></param>
        /// <returns></returns>
        public static string InitCstomesConstracts(List<Contracts> cons)
        {

            if (cons==null|| cons.Count <= 0) return string.Empty;
            StringBuilder str = new StringBuilder();
            cons.ForEach(item=>
            {
                str.AppendLine("[[genesis.system_cells]]");
                str.AppendLine("file = { file = \"cells/"+item.filename+"\" }");
                str.AppendLine("create_type_id = true");
                str.AppendLine("capacity = 100_000_0000_0000");
            });
            return str.ToString();
          
        }
    }
}
