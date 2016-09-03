using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using EnvDTE;
using EnvDTE80;
using Famoser.UWPTileGeneratorRevised.Enums;
using Famoser.UWPTileGeneratorRevised.Models.Configuration;
using Microsoft.VisualStudio.Shell;

namespace Famoser.UWPTileGeneratorRevised.Helpers
{
    internal class VisualStudioHelper : IDisposable
    {
        private readonly string _packageManifestPath;
        private readonly string _packageManifestFolder;
        private readonly string _selectedFilePath;
        private readonly Project _project;
        private readonly UIHierarchyItem _selectedFile;
        private readonly DTE2 _dte2Service;

        public VisualStudioHelper(DTE2 dte2Service)
        {
            _dte2Service = dte2Service;
            Cursor.Current = Cursors.WaitCursor;

            _selectedFile = ResolveSelectedItem();
            _selectedFilePath = ResolveSelectedFilePath();
            _project = ResolveContainingProject();
            _packageManifestPath = GetPackageManifestPath();
            _packageManifestFolder = _packageManifestPath.Substring(0, _packageManifestPath.LastIndexOf("\\", StringComparison.Ordinal));
        }

        private string ResolveSelectedFilePath()
        {
            var projItem = GetSelectedItem().Object as ProjectItem;
            return projItem?.Properties.Item("FullPath").Value.ToString();
        }

        private UIHierarchyItem ResolveSelectedItem()
        {
            var hierarchy = _dte2Service.ToolWindows.SolutionExplorer;
            var selectedItems = hierarchy.SelectedItems as IEnumerable<UIHierarchyItem>;
            return selectedItems?.FirstOrDefault();
        }

        private Project ResolveContainingProject()
        {
            var item = GetSelectedItem();
            var proj = item.Object as ProjectItem;
            return proj?.ContainingProject;
        }

        private string GetSolutionFileName(Tile tile)
        {
            return tile.BaseFileName + ".png";
        }

        private string GetPathForPackageManifest(Tile tile)
        {
            var filePath = GetSelectedItemPath();
            var fileFolder = filePath.Substring(0, filePath.LastIndexOf("\\", StringComparison.Ordinal));
            var relativePath = fileFolder.Replace(_packageManifestFolder, "").Substring(1);
            return relativePath + "\\" + GetSolutionFileName(tile);
        }

        private string GetPathForProjectFile(string absolutePath)
        {
            var folderPath = _project.FullName.Substring(_project.FullName.LastIndexOf("\\", StringComparison.Ordinal));
            return absolutePath.Replace(folderPath, "");
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
                logo.Value = GetPathForPackageManifest(tile);
            }

            var visualElemment = xdocument.Descendants(XName.Get("VisualElements", xmlNamespace)).FirstOrDefault();
            if (visualElemment != null)
            {
                if (tile.TileSize == TileSize.AppList || tile.TileSize == TileSize.Medium)
                {
                    visualElemment.SetAttributeValue(tile.XmlName, GetPathForPackageManifest(tile));
                }

                if (tile.TileSize == TileSize.Splash)
                {
                    var splashScreen = xdocument.Descendants(XName.Get("SplashScreen", xmlNamespace)).FirstOrDefault();
                    if (splashScreen == null)
                    {
                        visualElemment.Add(new XElement(XName.Get("SplashScreen", xmlNamespace)));
                        splashScreen = xdocument.Descendants(XName.Get("SplashScreen", xmlNamespace)).FirstOrDefault();
                    }

                    splashScreen?.SetAttributeValue("Image", GetPathForPackageManifest(tile));
                }

                if (tile.TileSize == TileSize.Small || tile.TileSize == TileSize.Wide || tile.TileSize == TileSize.Large)
                {
                    var defaultTitle = xdocument.Descendants(XName.Get("DefaultTile", xmlNamespace)).FirstOrDefault();
                    if (defaultTitle == null)
                    {
                        visualElemment.Add(new XElement(XName.Get("DefaultTile", xmlNamespace)));
                        defaultTitle = xdocument.Descendants(XName.Get("DefaultTile", xmlNamespace)).FirstOrDefault();
                    }

                    defaultTitle?.SetAttributeValue(tile.XmlName, GetPathForPackageManifest(tile));
                }
            }


            xdocument.Save(_packageManifestPath);
        }

        public void AddFileToProject(string savePath)
        {
            _project.ProjectItems.AddFromFile(GetPathForProjectFile(savePath));
        }

        private UIHierarchyItem GetSelectedItem()
        {
            return _selectedFile;
        }

        public string GetSelectedItemPath()
        {
            return _selectedFilePath;
        }

        /// <summary>
        /// Finds the package manifest.
        /// </summary>
        private string GetPackageManifestPath()
        {
            var item = GetSelectedItem();
            string path = null;
            while (path == null && item != null)
            {
                path = GetPackageManifestPath(item.Collection);
                item = item.Collection.Parent as UIHierarchyItem;
            }
            return path;
        }

        private string GetPackageManifestPath(UIHierarchyItems items)
        {
            foreach (var uiHierarchyItem in items)
            {
                var typedItem = uiHierarchyItem as UIHierarchyItem;
                if (typedItem != null && typedItem.Name.ToLower().Equals("package.appxmanifest"))
                {
                    var projectItem = typedItem.Object as ProjectItem;

                    return projectItem?.Properties.Item("FullPath").Value.ToString();
                }
            }
            return null;
        }

        public void Dispose()
        {
            _project.Save();
            Cursor.Current = Cursors.Default;
        }
    }
}
