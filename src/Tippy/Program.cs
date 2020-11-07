using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Tippy.Ctrl;

namespace Tippy
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);
            Core.Environment.CreateAppDataFolder();
            ProcessManager.Start();
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        static void OnAppExit(object sender, EventArgs e)
        {
            Console.WriteLine("Exiting Tippy...");
            ProcessManager.Stop();
        }
    }
}
