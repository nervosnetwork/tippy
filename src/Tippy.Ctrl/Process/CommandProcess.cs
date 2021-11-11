using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using Tippy.Core.Models;

namespace Tippy.Ctrl.Process
{
    delegate void LogEventHandler(object? sender, LogReceivedEventArgs e);

    abstract class CommandProcess
    {
        internal ProcessInfo ProcessInfo { get; private set; }
        protected System.Diagnostics.Process? process;
        public event LogEventHandler? LogReceived;

        internal CommandProcess(ProcessInfo processInfo) => ProcessInfo = processInfo;

        protected abstract void Configure();

        public bool IsRunning
        {
            get
            {
                if (process != null && !process.HasExited)
                {
                    process.Refresh();
                    return process.Responding;
                }
                return false;
            }
        }

        public void Start()
        {

            if (!Directory.Exists(WorkingDirectory()))
            {
                Directory.CreateDirectory(WorkingDirectory());
            }

            Stop();

            Configure();

            HandleOutput();
            process?.Start();

            process?.BeginErrorReadLine();
            process?.BeginOutputReadLine();
        }

        public void Stop()
        {
            if (process != null)
            {
                process.Kill();
                process.WaitForExit();
                process = null;
            }
        }

        /// <summary>
        /// copy the constracts files into the ckb working scripts directory
        /// </summary>
        protected void CopyTheConstractFiles(List<Contracts> paths)
        {
            var specs_cells_path = WorkingScriptDirectory();
            if (!Directory.Exists(specs_cells_path))
            {
                Directory.CreateDirectory(specs_cells_path);
                try
                {
                    foreach (var item in paths)
                    {
                        if (File.Exists(item.filepath))
                        {
                            File.Copy(item.filepath, Path.Combine(specs_cells_path, item.filename));

                        }
                    }
                 }
                catch (Exception e)
                {

                }
            }
        }


       

        protected void HandleOutput()
        {
            if (process == null)
            {
                return;
            }
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;

            process.OutputDataReceived += (sender, e) =>
            {
                LogReceived?.Invoke(
                    this,
                    new LogReceivedEventArgs()
                    {
                        ID = ProcessInfo.ID,
                        Log = e.Data
                    });
            };

            process.ErrorDataReceived += (sender, e) =>
            {
                LogReceived?.Invoke(
                    this,
                    new LogReceivedEventArgs()
                    {
                        ID = ProcessInfo.ID,
                        Log = e.Data
                    });
            };
        }


        internal  string WorkingDirectory() => WorkPathManage.WorkingDirectory(ProcessInfo.ID);



        /// <summary>
        /// set the scripts directory
        /// </summary>
        /// <returns></returns>
        internal string ScriptDirectory() => WorkPathManage.BinDepsPath();

        /// <summary>
        /// set the working scripts  directory
        /// </summary>
        /// <returns></returns>
        internal  string WorkingScriptDirectory() => WorkPathManage.WorkingScriptDirectory(ProcessInfo.ID);


        internal static string BinaryFullPath(string binary) => WorkPathManage.BinaryFullPath(binary);




        internal static string[] BinDepsDirectory()
        {
            return WorkPathManage.BinDepsDirectory();
        }

        internal static string BinDepsPath(string childDirectory = "")
        {
            return WorkPathManage.BinDepsPath(childDirectory);

        }

    }
}
