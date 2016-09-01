using System;
using System.IO;
using System.Net.Mime;
using Famoser.UWPTileGeneratorRevised.Business.Models.Configuration;
using Svg;

namespace Famoser.UWPTileGeneratorRevised.Business.Helpers
{
    internal class ImageHelper
    {
        private readonly string _saveFolder;
        private readonly string _sourceFilePath;

        public ImageHelper(string sourceFilePath)
        {
            _sourceFilePath = sourceFilePath;
            _saveFolder = Path.GetDirectoryName(sourceFilePath);
        }

        /// <summary>
        /// Generates the tiles.
        /// </summary>
        /// <returns></returns>
        public void GenerateFile(Tile tile, double scaleFactor, string savePath)
        {
            var extension = Path.GetExtension(_sourceFilePath);
            if (extension == ".png")
            {
                using (var originalImage = MediaTypeNames.Image.FromFile(_sourceFilePath))
                {
                    using (var resizedImage = ResizeImage((Bitmap)originalImage, new Size((int)(tile.Width * scaleFactor), (int)(tile.Height * scaleFactor)), tile.IconWidth, tile.IconHeight))
                    {
                        resizedImage.Save(savePath);
                    }
                }
            }
            else if (extension == ".svg")
            {
                using (var resizedImage = ResizeImage(SvgDocument.Open(_sourceFilePath), new Size((int)(tile.Width * scaleFactor), (int)(tile.Height * scaleFactor)), tile.IconWidth, tile.IconHeight))
                {
                    resizedImage.Save(savePath);
                }
            }
        }
        
        public static Image ResizeImage(SvgDocument image, Size size, double iconWidth, double iconHeight)
        {
            var originalImageSize = new Size((int)image.Width.Value, (int)image.Height.Value);
            return ResizeImage((newImage, x, y, width, height) =>
            {
                using (Graphics graphicsHandle = Graphics.FromImage(newImage))
                {
                    graphicsHandle.InterpolationMode = InterpolationMode.Default;
                    using (var svgImage = new Bitmap(width, height))
                    {
                        image.Draw(svgImage);
                        svgImage.MakeTransparent();
                        graphicsHandle.DrawImage(svgImage, new Rectangle(x, y, width, height));
                    }
                }
            },
            originalImageSize,
            size,
            iconWidth,
            iconHeight);
        }

        public static Image ResizeImage(Bitmap image, Size size, double iconWidth, double iconHeight)
        {
            var originalImageSize = new Size(image.Width, image.Height);
            return ResizeImage((newImage, x, y, width, height) =>
            {
                var firstPixel = image.GetPixel(0, 0);
                var brush = new SolidBrush(firstPixel);

                using (var graphicsHandle = Graphics.FromImage(newImage))
                {
                    graphicsHandle.InterpolationMode = InterpolationMode.Default;
                    graphicsHandle.FillRectangle(brush, new Rectangle(0, 0, size.Width, size.Height));
                    graphicsHandle.DrawImage(image, x, y, width, height);
                }
            },
            originalImageSize,
            size,
            iconWidth,
            iconHeight);
        }
        
        private static Bitmap ResizeImage(Action<Bitmap, int, int, int, int> action, Size originalImageSize, Size requestedSize, double iconWidth, double iconHeight)
        {
            var originalWidth = originalImageSize.Width;
            var originalHeight = originalImageSize.Height;

            var percentWidth = requestedSize.Width / (float)originalWidth;
            var percentHeight = requestedSize.Height / (float)originalHeight;
            var percent = percentHeight < percentWidth ? percentHeight : percentWidth;

            var newWidth = (int)(originalWidth * percent * iconWidth);
            var newHeight = (int)(originalHeight * percent * iconHeight);
            
            var xPosition = (requestedSize.Width - newWidth) / 2;
            var yPosition = (requestedSize.Height - newHeight) / 2;

            var newImage = new Bitmap(requestedSize.Width, requestedSize.Height);

            action(newImage, xPosition, yPosition, newWidth, newHeight);

            return newImage;
        }
    }
}
