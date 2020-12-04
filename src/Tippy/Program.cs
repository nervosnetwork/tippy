using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tippy.Core;
using Tippy.Ctrl;
using Tippy.Hubs;
using Tippy.Util;

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

            if (Settings.GetSettings().AppSettings.OpenBrowserOnLaunch)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    UrlOpener.Open("http://localhost:5000/home");
                });
            }

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
            try
            {
                var context = services.GetRequiredService<Core.Data.DbContext>();
                context.Database.EnsureCreated();
            }
            catch (Exception ex)
            {
                var logger = services.GetRequiredService<ILogger<Program>>();
                logger.LogError(ex, "An error occurred creating the DB.");
            }
        }

        static void OnAppExit(object? sender, EventArgs e)
        {
            Console.WriteLine("Exiting Tippy...");
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
