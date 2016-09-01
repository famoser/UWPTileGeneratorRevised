using System.Collections.Generic;
using Famoser.UWPTileGeneratorRevised.Business.Enums;

namespace Famoser.UWPTileGeneratorRevised.Business.Models.Configuration
{
    internal class Tile
    {
        public string BaseFileName { get; set; }
        public string XmlName { get; set; }
        public int Height { get; set; }
        public int Width { get; set; }

        public double IconHeight { get; set; }
        public double IconWidth { get; set; }

        public List<double> ScaleFactors { get; set; }

        public TileSize TileSize { get; set; }
    }
}
