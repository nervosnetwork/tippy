using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Tippy.Ctrl;
using Tippy.Hubs;

namespace Tippy
{
    public class Program
    {
        static IHubContext<LogHub> _hubContext;

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.ProcessExit += new EventHandler(OnAppExit);
            Core.Environment.CreateAppDataFolder();

            var host = CreateHostBuilder(args).Build();
            CreateDbIfNotExists(host);

            _hubContext = (IHubContext<LogHub>)host.Services.GetService(typeof(IHubContext<LogHub>));
            ProcessManager.NodeLogReceived += new NodeLogEventHandler(OnLogReceived);

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

        static void OnAppExit(object sender, EventArgs e)
        {
            Console.WriteLine("Exiting Tippy...");
            ProcessManager.Stop();
        }

        static async void OnLogReceived(object? sender, LogReceivedEventArgs e)
        { 
            await _hubContext.Clients.All.SendAsync("ReceiveLog", e.Log);
        }
    }
}
