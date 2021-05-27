using System;
using System.Diagnostics;

namespace Tippy.Util
{
    public class UrlOpener
    {
        // Ref: https://stackoverflow.com/questions/14982746/open-a-browser-with-a-specific-url-by-console-application
        public static void Open(string url)
        {
            try
            {
                if (OperatingSystem.IsWindows())
                {
                    url = url.Replace("&", "^&");
                    Process.Start(new ProcessStartInfo("cmd", $"/c start {url}"));
                }
                else if (OperatingSystem.IsLinux())
                {
                    Process.Start("xdg-open", url);
                }
                else if (OperatingSystem.IsMacOS())
                {
                    Process.Start("open", url);
                }
            }
            catch
            {
                Console.WriteLine($"Couldn't open URL $(url). Try opening it from your browser.");
            }
        }
    }
}
