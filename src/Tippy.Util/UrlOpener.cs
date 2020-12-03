using System.Diagnostics;
using System.Runtime.InteropServices;

namespace Tippy.Util
{
    public class UrlOpener
    {
        // Ref: https://stackoverflow.com/questions/14982746/open-a-browser-with-a-specific-url-by-console-application
        public static void Open(string url)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                url = url.Replace("&", "^&");
                Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                Process.Start("xdg-open", url);
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                Process.Start("open", url);
            }
        }
    }
}
