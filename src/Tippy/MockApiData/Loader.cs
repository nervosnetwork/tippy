using System;
using System.IO;
using Microsoft.Extensions.FileProviders;
using Newtonsoft.Json;

namespace Tippy.MockApiData
{
    public class Loader
    {
        internal static string LoadFile(string filename)
        {
            var provider = new ManifestEmbeddedFileProvider(typeof(Program).Assembly, "MockApiData");
            var fileInfo = provider.GetFileInfo(/*"Tippy.MockApiData." + )*/filename + ".json");
            using StreamReader reader = new StreamReader(fileInfo.CreateReadStream());
            return reader.ReadToEnd();
        }

        internal static string JsonFromFile(string filename)
        {
            var content = LoadFile(filename);
            return JsonConvert.SerializeObject(JsonConvert.DeserializeObject(content));
        }
    }
}
