using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.UWPTileGeneratorRevised.Models.Configuration;
using Newtonsoft.Json;

namespace Famoser.UWPTileGeneratorRevised.Helpers
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
