using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE80;
using Famoser.UWPTileGeneratorRevised.Enums;
using Famoser.UWPTileGeneratorRevised.Models.Configuration;
using Microsoft.VisualStudio.Shell;

namespace Famoser.UWPTileGeneratorRevised.Helpers
{
    internal class VisualStudioHelper
    {
        private readonly string _packageManifestPath;
        private readonly string _relativeFilePath;
        public VisualStudioHelper(string relativeFilePath, DTE2 dte2Service)
        {
            _relativeFilePath = relativeFilePath;

            _packageManifestPath = GetPackageManifestPath(dte2Service);
        }

        private string GetSolutionFileName(Tile tile)
        {
            return tile.BaseFileName + ".png";
        }

        private string GetSolutionPath(Tile tile)
        {
            return _relativeFilePath + GetSolutionFileName(tile);
        }

        public void AddTileToPackage(Tile tile)
        {
            var xdocument = XDocument.Parse(File.ReadAllText(_packageManifestPath));
            var xmlNamespace = "http://schemas.microsoft.com/appx/manifest/uap/windows10";
            var defaultNamespace = "http://schemas.microsoft.com/appx/manifest/foundation/windows10";

            if (tile.TileSize == TileSize.StoreLogo)
            {
                //add logo
                var logo = xdocument.Descendants(XName.Get(tile.XmlName, defaultNamespace)).First();
                logo.Value = GetSolutionPath(tile);
            }

            var visualElemment = xdocument.Descendants(XName.Get("VisualElements", xmlNamespace)).FirstOrDefault();
            if (visualElemment != null)
            {
                if (tile.TileSize == TileSize.AppList || tile.TileSize == TileSize.Medium)
                {
                    visualElemment.SetAttributeValue(tile.XmlName, GetSolutionPath(tile));
                }

                if (tile.TileSize == TileSize.Splash)
                {
                    var splashScreen = xdocument.Descendants(XName.Get("SplashScreen", xmlNamespace)).FirstOrDefault();
                    if (splashScreen == null)
                    {
                        visualElemment.Add(new XElement(XName.Get("SplashScreen", xmlNamespace)));
                        splashScreen = xdocument.Descendants(XName.Get("SplashScreen", xmlNamespace)).FirstOrDefault();
                    }

                    splashScreen?.SetAttributeValue(tile.XmlName, GetSolutionPath(tile));
                }

                if (tile.TileSize == TileSize.Small || tile.TileSize == TileSize.Wide || tile.TileSize == TileSize.Large)
                {
                    var defaultTitle = xdocument.Descendants(XName.Get("DefaultTile", xmlNamespace)).FirstOrDefault();
                    if (defaultTitle == null)
                    {
                        visualElemment.Add(new XElement(XName.Get("DefaultTile", xmlNamespace)));
                        defaultTitle = xdocument.Descendants(XName.Get("DefaultTile", xmlNamespace)).FirstOrDefault();
                    }

                    defaultTitle?.SetAttributeValue(tile.XmlName, GetSolutionPath(tile));
                }
            }


            xdocument.Save(_packageManifestPath);
        }

        /// <summary>
        /// Finds the package manifest.
        /// </summary>
        private string GetPackageManifestPath(DTE2 dte2Service)
        {
            var hierarchy = dte2Service.ToolWindows.SolutionExplorer;
            var solutionRoot = hierarchy.UIHierarchyItems.Item(1);

            for (var i = 1; i <= solutionRoot.UIHierarchyItems.Count; i++)
            {
                var uiHierarchyItems = solutionRoot.UIHierarchyItems.Item(i).UIHierarchyItems;

                foreach (UIHierarchyItem uiHierarchy in uiHierarchyItems)
                {
                    if (!uiHierarchy.Name.ToLower().Equals("package.appxmanifest")) continue;

                    var projectItem = uiHierarchy.Object as ProjectItem;
                    return projectItem?.Properties.Item("FullPath").Value.ToString();
                }
            }
            return null;
        }
    }
}
