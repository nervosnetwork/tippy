using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace Tippy.Core
{
    public class Environment
    {
        public static string GetAppDataFolder() => Path.Combine(GetSystemAppDataFolder(), "Tippy");

        public static void CreateAppDataFolder()
        {
            if (Directory.Exists(GetAppDataFolder()))
            {
                return;
            }

            try
            {
                Directory.CreateDirectory(GetAppDataFolder());
            }
            catch (Exception ex)
            {
                Debug.Fail($"Failed to create app data folder {ex}");
            }
        }

        private static string GetSystemAppDataFolder()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var home = System.Environment.GetFolderPath(System.Environment.SpecialFolder.UserProfile);
                return Path.Combine(home, "Library", "Application Support");
            }

            return System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);
        }
    }
}
