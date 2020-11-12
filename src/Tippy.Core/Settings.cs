using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text.Json.Serialization;
using System.Reflection;
using System.IO;

namespace Tippy.Core
{
    public class Settings
    {
        private static Lazy<IConfiguration> Configuration = new Lazy<IConfiguration>(() =>
        {
            if (!File.Exists(FilePath))
            {
                SaveText(SettingsTemplate);
            }

            var builder = new ConfigurationBuilder()
              .SetBasePath(Environment.GetAppDataFolder())
              .AddJsonFile("settings.json", false, true);

            return builder.Build();
        });

        private static IConfiguration Config => Configuration.Value;

        public static BlockAssembler BlockAssembler => Config.GetSection("blockAssembler").Get<BlockAssembler>();

        static void Save()
        { 
            // TODO
        }

        static void SaveText(string text)
        {
            File.WriteAllText(FilePath, text);
        }

        private static string FilePath = Path.Combine(Environment.GetAppDataFolder(), "settings.json");

        private static string SettingsTemplate
        {
            get
            {
                var resourceName = "Tippy.Core.Settings.json";
                using Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName);
                using StreamReader reader = new StreamReader(stream);
                return reader.ReadToEnd();
            }
        }
    }

    public class BlockAssembler
    {
        [JsonPropertyName("lockArg")]
        public String LockArg { get; set; }
    }
}
