using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using EnvDTE;
using EnvDTE80;
using Famoser.UWPTileGeneratorRevised.Enums;
using Famoser.UWPTileGeneratorRevised.Helpers;
using Famoser.UWPTileGeneratorRevised.Models.Configuration;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Famoser.UWPTileGeneratorRevised.Workflow
{
    public class GenerateTilesWorkflow
    {
        private readonly List<Tile> _tilesToGenerate = new List<Tile>();
        private static ConfigurationRoot _config;
        private IVsOutputWindowPane _outputWindow;
        private DTE2 _dte2Service;

        public GenerateTilesWorkflow(IServiceProvider serviceProvider, params ActionType[] actions)
        {
            ResolverServices(serviceProvider);
            ResolveConfig();
            ResolveActions(actions);
        }

        private void ResolverServices(IServiceProvider serviceProvider)
        {
            _outputWindow = (IVsOutputWindowPane)serviceProvider.GetService(typeof(SVsGeneralOutputWindowPane));
            _outputWindow.OutputString("Resolving services... \n");
            _dte2Service = (DTE2)serviceProvider.GetService(typeof(DTE));
        }

        private void ResolveConfig()
        {
            _outputWindow.OutputString("Resolving config... \n");

            if (_config != null)
                return;

            var helper = new ConfigurationHelper();
            _config = helper.GetConfiguration();
        }

        private void ResolveActions(ActionType[] actions)
        {
            _outputWindow.OutputString("Resolving actions... \n");
            var tileImages = new List<TileSize>()
            {
                TileSize.AppList,
                TileSize.Large,
                TileSize.Medium,
                TileSize.Small,
                TileSize.Wide
            };
            var storeLogo = new List<TileSize>()
            {
                TileSize.StoreLogo
            };
            var splashScreen = new List<TileSize>()
            {
                TileSize.Splash
            };
            foreach (var actionType in actions)
            {
                List<Tile> toGenerate = null;
                switch (actionType)
                {
                    case ActionType.GenerateTileImages:
                        toGenerate = _config.TileSizes.Where(s => tileImages.Any(t => t == s.TileSize)).ToList();
                        break;
                    case ActionType.GenerateStoreLogo:
                        toGenerate = _config.TileSizes.Where(s => storeLogo.Any(t => t == s.TileSize)).ToList();
                        break;
                    case ActionType.GenerateSplashImages:
                        toGenerate = _config.TileSizes.Where(s => splashScreen.Any(t => t == s.TileSize)).ToList();
                        break;
                }
                if (toGenerate != null)
                    foreach (var tile in toGenerate)
                    {
                        if (!_tilesToGenerate.Contains(tile))
                            _tilesToGenerate.Add(tile);
                    }
            }
        }

        //private void ResolveSourceFile()
        //{
        //    _outputWindow.OutputString("Resolving source file... \n");
            
        //    var hierarchy = _dte2Service.ToolWindows.SolutionExplorer;
        //    var selectedItems = (Array)hierarchy.SelectedItems;
        //    var selectedItem = selectedItems.Length > 0 ? (UIHierarchyItem)selectedItems.GetValue(0) : null;

        //    if (selectedItem != null)
        //    {
        //        var projectItem = selectedItem.Object as ProjectItem;
        //        _sourceFileName = projectItem?;
        //        _project = projectItem?.ContainingProject;
        //    }
        //}

        public string GetLastError()
        {
            return _lastError;
        }

        private bool ReturnFailed(string reason)
        {
            _lastError = reason;
            _outputWindow.OutputString(_lastError);
            return false;
        }

        private string _lastError;
        public bool DoWork()
        {
            using (var vsHelper = new VisualStudioHelper(_dte2Service))
            {
                if (!vsHelper.CanAccessPackageManifest())
                {
                    return ReturnFailed("No package manifest found... \n");
                }
                var sfn = vsHelper.GetSelectedItemPath();
                var imageHelper = new ImageHelper(sfn);
                if (string.IsNullOrEmpty(sfn))
                {
                    return ReturnFailed("No file selected, aborting... \n");
                }

                var extension = Path.GetExtension(sfn);
                if (extension != "png" && extension != "svg")
                {
                    return ReturnFailed("Unsupported file selected (only .png & .svg allowed), aborting... \n");
                }

                _outputWindow.OutputString("Starting generation of images... \n");


                foreach (var tile in _tilesToGenerate)
                {
                    _outputWindow.OutputString("Generating " + tile.XmlName + "... \n");
                    foreach (var scaleFactor in tile.ScaleFactors)
                    {
                        var savePath = GenerateSavePath(tile, scaleFactor, sfn);
                        imageHelper.GenerateFile(tile, scaleFactor, savePath);
                        vsHelper.AddFileToProject(savePath);
                        vsHelper.AddTileToPackage(tile);
                    }
                }
                _outputWindow.OutputString("Saving project... \n");
            }
            return true;
        }

        private string GenerateSavePath(Tile tile, double scaleFactor, string sourceFilePath)
        {
            var fileName = tile.BaseFileName + ".scale-" + scaleFactor * 100 + ".png";
            return Path.Combine(Path.GetDirectoryName(sourceFilePath), fileName);
        }
    }
}
