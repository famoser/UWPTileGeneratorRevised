using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Famoser.UWPTileGeneratorRevised.Enums;

namespace Famoser.UWPTileGeneratorRevised.Models.Configuration
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
