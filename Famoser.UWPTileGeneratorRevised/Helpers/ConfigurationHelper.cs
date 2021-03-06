﻿using Famoser.UWPTileGeneratorRevised.Models.Configuration;
using Newtonsoft.Json;

namespace Famoser.UWPTileGeneratorRevised.Helpers
{
    internal class ConfigurationHelper
    {
        public ConfigurationRoot GetConfiguration()
        {
            var json = @"{
  ""TileSizes"": [
    {
      ""BaseFileName"": ""Square44x44Logo"",
      ""XmlName"": ""Square44x44Logo"",
      ""Height"": 44,
      ""Width"": 44,
      ""IconHeight"": 0.75,
      ""IconWidth"": 0.75,
      ""ScaleFactors"": [ 1, 1.25, 1.5, 2, 4 ],
      ""TileSize"": 0
    },
    {
      ""BaseFileName"": ""Square71x71Logo"",
      ""XmlName"": ""Square71x71Logo"",
      ""Height"": 71,
      ""Width"": 71,
      ""IconHeight"": 0.5,
      ""IconWidth"": 0.5,
      ""ScaleFactors"": [ 1, 1.25, 1.5, 2, 4 ],
      ""TileSize"": 1
    },
    {
      ""BaseFileName"": ""Square150x150Logo"",
      ""XmlName"": ""Square150x150Logo"",
      ""Height"": 150,
      ""Width"": 150,
      ""IconHeight"": 0.33,
      ""IconWidth"": 0.33,
      ""ScaleFactors"": [ 1, 1.25, 1.5, 2, 4 ],
      ""TileSize"": 2
    },
    {
      ""BaseFileName"": ""Wide310x150Logo"",
      ""XmlName"": ""Wide310x150Logo"",
      ""Height"": 150,
      ""Width"": 310,
      ""IconHeight"": 0.33,
      ""IconWidth"": 0.16,
      ""ScaleFactors"": [ 1, 1.25, 1.5, 2, 4 ],
      ""TileSize"": 3
    },
    {
      ""BaseFileName"": ""Square310x310Logo"",
      ""XmlName"": ""Square310x310Logo"",
      ""Height"": 310,
      ""Width"": 310,
      ""IconHeight"": 0.33,
      ""IconWidth"": 0.33,
      ""ScaleFactors"": [ 1, 1.25, 1.5, 2, 4 ],
      ""TileSize"": 4
    },
    {
      ""BaseFileName"": ""SplashScreen"",
      ""XmlName"": ""SplashScreen"",
      ""Height"": 300,
      ""Width"": 620,
      ""IconHeight"": 0.33,
      ""IconWidth"": 0.16,
      ""ScaleFactors"": [ 1, 1.25, 1.5, 2, 4 ],
      ""TileSize"": 5
    },
    {
      ""BaseFileName"": ""StoreLogo"",
      ""XmlName"": ""Logo"",
      ""Height"": 50,
      ""Width"": 50,
      ""IconHeight"": 0.5,
      ""IconWidth"": 0.5,
      ""ScaleFactors"": [ 1, 1.25, 1.5, 2, 4 ],
      ""TileSize"": 6
    }
  ]
}";
            return JsonConvert.DeserializeObject<ConfigurationRoot>(json);
        }
    }
}
