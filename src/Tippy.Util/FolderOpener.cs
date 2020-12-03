using System.Diagnostics;

namespace Tippy.Util
{
    public class FolderOpener
    {
        public static void Open(string folder)
        {
            Process.Start(new System.Diagnostics.ProcessStartInfo()
            {
                FileName = folder,
                UseShellExecute = true,
                Verb = "open"
            });
        }
    }
}
