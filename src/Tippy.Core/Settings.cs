using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Reflection;
using System.IO;
using System.Text.Json;

namespace Tippy.Core
{
    public class Settings
    {
        private static Settings? Singleton;
        public static Settings GetSettings()
        {
            if (Singleton == null)
            {
                if (!File.Exists(FilePath))
                {
                    SaveJson(SettingsTemplate);
                }

                Singleton = new();
                var config = new ConfigurationBuilder()
                    .SetBasePath(Environment.GetAppDataFolder())
                    .AddJsonFile("settings.json", false, true)
                    .Build();
                config.Bind(Singleton);

            }

            return Singleton;
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
}
