using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tippy.Ctrl;

namespace Tippy.Shared.MyConstracts
{

    public class Domain_ConstractFile
    {
        /// <summary>
        /// filename
        /// </summary>

        public string filename { get; set; }

        /// <summary>
        /// filecreate time
        /// </summary>
        public DateTime filecreatetime { get; set; }

        /// <summary>
        /// file modify time
        /// </summary>

        public DateTime modifytime { get; set; }

        /// <summary>
        ///  file path
        /// </summary>
        public string filepath { get; set; }

        /// <summary>
        ///  that is seleced for into ckb constract
        /// </summary>

        public bool selectforintockb { get; set; } = false;



        /// <summary>
        /// domain: return  List<Domain_ConstractFile>
        /// </summary>
        /// <returns></returns>
        public List<Domain_ConstractFile> GetConstractFiles()
        {
            List<Domain_ConstractFile> list = new List<Domain_ConstractFile>();
            WorkPathManage.GetListOfConstractFiles().ForEach(item =>
            {

                list.Add(new Domain_ConstractFile()
                {
                    filename = item.Name,
                    filepath = item.FullName,
                    filecreatetime = item.CreationTime,
                    modifytime = item.LastWriteTime,
                });
            });
            return list;
        }
    }

}


    
