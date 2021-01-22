using System.IO;
using System.Reflection;
using System.Text.Json;
using Microsoft.Extensions.Configuration;

namespace Tippy.Core
{
    public class Settings
    {
        public AppSettings AppSettings { get; set; } = default!;

        private static Settings? singleton;
        public static Settings GetSettings()
        {
            if (singleton == null)
            {
                if (!File.Exists(FilePath))
                {
                    SaveJson(SettingsTemplate);
                }

                singleton = new();
                var config = new ConfigurationBuilder()
                    .SetBasePath(Environment.GetAppDataFolder())
                    .AddJsonFile("settings.json", false, true)
                    .Build();
                config.Bind(singleton);

            }

            return singleton;
        }

        public void Save()
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true
            };
            var json = JsonSerializer.Serialize(GetSettings(), options);
            SaveJson(json);
        }

        static void SaveJson(string json)
        {
            File.WriteAllText(FilePath, json);
        }

        private static readonly string FilePath = Path.Combine(Environment.GetAppDataFolder(), "settings.json");

        private static string SettingsTemplate
        {
            get
            {
                var resourceName = "Tippy.Core.Settings.json";
                using Stream? stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                if (stream != null)
                {
                    using StreamReader reader = new(stream);
                    return reader.ReadToEnd();
                }
                return "";
            }
        }
    }

    public class AppSettings
    {
        public bool OpenBrowserOnLaunch { get; set; }
    }
}
