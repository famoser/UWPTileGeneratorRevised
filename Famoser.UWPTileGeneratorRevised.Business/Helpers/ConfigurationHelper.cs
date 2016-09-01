using System;
using System.IO;
using Famoser.UWPTileGeneratorRevised.Business.Models.Configuration;
using Newtonsoft.Json;

namespace Famoser.UWPTileGeneratorRevised.Business.Helpers
{
    internal class ConfigurationHelper
    {
        public ConfigurationRoot GetConfiguration()
        {
            var json = File.ReadAllText(Path.Combine(Environment.CurrentDirectory, "Assets/tiles.json"));
            return JsonConvert.DeserializeObject<ConfigurationRoot>(json);
        }
    }
}
