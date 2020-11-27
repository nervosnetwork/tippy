using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;

namespace Tippy.Util
{
    public class LocalPort
    {
        public static List<int> PortsInUse()
        {
            var ipGlobalProperties = IPGlobalProperties.GetIPGlobalProperties();

            var connections = ipGlobalProperties.GetActiveTcpConnections().Select(c => c.LocalEndPoint);
            var tcpListeners = ipGlobalProperties.GetActiveTcpListeners();
            var udpListeners = ipGlobalProperties.GetActiveUdpListeners();
            return connections.Concat(tcpListeners)
                .Concat(udpListeners)
                .Select(e => e.Port)
                .ToList();
        }
    }
}
