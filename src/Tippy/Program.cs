using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tippy.Ctrl;
using Tippy.Hubs;

namespace Tippy
{
    public class Program
    {
        static IHubContext<LogHub>? _hubContext;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += OnAppExit;
            Core.Environment.CreateAppDataFolder();

            var host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);

            _hubContext = host.Services.GetService(typeof(IHubContext<LogHub>)) as IHubContext<LogHub>;
            ProcessManager.NodeLogReceived += OnNodeLogReceived;
            ProcessManager.FetchInfo();

            host.Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                });

        private static void CreateDbIfNotExists(IHost host)
        {
           
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var logger = services.GetRequiredService<ILogger<Program>>();
            logger.LogWarning("the working path: "+WorkPathManage.WorkingScriptDirectory(0));
            try
            {
                var context = services.GetRequiredService<Core.Data.TippyDbContext>();
                context.Database.Migrate();
                
                
            }
            catch (Exception ex)
            {
            

                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        static void OnAppExit(object? sender, EventArgs e)
        {
            Console.WriteLine("Exiting Tippy...");
            string ss = "";
            ProcessManager.Stop();
        }

        static async void OnNodeLogReceived(object? sender, LogReceivedEventArgs e)
        {
            if (_hubContext != null)
            {
                await _hubContext.Clients.All.SendAsync("ReceiveLog", e.ID, e.Log);
            }
        }
    }
}
