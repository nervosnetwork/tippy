using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace Tippy.Ctrl
{

    public enum ckbenum
    {
        ckb,
        ckbindexer,
        ckbdebugger

    }
    /// <summary>
    /// work directory manange
    /// </summary>
    public class WorkPathManage
    {
        internal static string WorkingDirectory(int id) =>
         Path.Combine(Core.Environment.GetAppDataFolder(), $"chain-{id}");
        
      

        /// <summary>
        /// set the scripts directory
        /// </summary>
        /// <returns></returns>
        public static string ScriptDirectory() => BinDepsPath("scripts");

        /// <summary>
        /// set the working scripts  directory
        /// </summary>
        /// <returns></returns>
        public static string WorkingScriptDirectory(int id) => Path.Combine(new string[] { WorkingDirectory(id), "specs", "cells" });


        internal static string BinaryFullPath(string binary) =>
            Path.Combine(Path.Combine(BinDepsDirectory()), binary);




        internal static string[] BinDepsDirectory()
        {
            var platformFolder = "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                platformFolder = "mac";
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                platformFolder = "linux";
            }

            return new[] { AppContext.BaseDirectory, "BinDeps", platformFolder };
        }

        public static string  CkbForPaltform(ckbenum ckb)
        {
            //"ckb", "ckb-indexer", "ckb-debugger"
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX) || RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return ckb switch
                {
                    ckbenum.ckb=>"ckb",
                    ckbenum.ckbdebugger=> "ckb-debugger",
                    ckbenum.ckbindexer=> "ckb-indexer",
                    _=> "ckb"
                };


            }
            else
            {
                return ckb switch
                {
                    ckbenum.ckb => "ckb.exe",
                    ckbenum.ckbdebugger => "ckb-debugger.exe",
                    ckbenum.ckbindexer => "ckb-indexer.exe",
                    _ => "ckb.exe"
                };
            }
         }





        internal static string BinDepsPath(string childDirectory = "")
        {
            if (!string.IsNullOrEmpty(childDirectory))
            {
                return Path.Combine(new[] { AppContext.BaseDirectory, "BinDeps", childDirectory });
            }
            else
            {
                return Path.Combine(new[] { AppContext.BaseDirectory, "BinDeps" });
            }

        }

        /// <summary>
        /// get the the list of constract files. 
        /// </summary>
        public static List<FileSystemInfo> GetListOfConstractFiles()
        {

            var specs_cells_path = WorkPathManage.ScriptDirectory();
            List<FileSystemInfo> list = new List<FileSystemInfo>();
            if (Directory.Exists(specs_cells_path))
            {
               

                try
                {
                    DirectoryInfo dir = new DirectoryInfo(ScriptDirectory());
                    FileSystemInfo[] fileinfo = dir.GetFileSystemInfos();
                    foreach (FileSystemInfo i in fileinfo)
                    {
                        if (!(i is DirectoryInfo))
                        {
                            list.Add(i);

                        }
                        continue;

                    }

                }
                catch (Exception e)
                {

                }

            }
            return list;
        }

    }
}
