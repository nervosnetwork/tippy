using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

namespace Tippy.Hubs
{
    public class LogHub : Hub
    {
        // Not used
        public async Task SendLog(string log)
        {
            await Clients.All.SendAsync("ReceiveLog", log);
        }
    }
}
