using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Hosting;
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
            _hubContext = (IHubContext<LogHub>)host.Services.GetService(typeof(IHubContext<LogHub>));
            ProcessManager.NodeLogReceived += new NodeLogEventHandler(OnLogReceived);
            ProcessManager.Start();
            host.Run();
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

        static async void OnLogReceived(object? sender, LogReceivedEventArgs e)
        { 
            await _hubContext.Clients.All.SendAsync("ReceiveLog", e.Log);
        }
    }
}
