using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Threading.Tasks;
using Famoser.FrameworkEssentials.Singleton;
using Famoser.UWPTileGeneratorRevised.Enums;
using Famoser.UWPTileGeneratorRevised.Workflow;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using Svg;

namespace Famoser.UWPTileGeneratorRevised
{
    internal class TileGeneratorsCommand
    {
        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// UWP tile command identifier.
        /// </summary>
        public const int UwpTileCommandId = 0x0100;

        /// <summary>
        /// The uwp splash command identifier.
        /// </summary>
        public const int UwpSplashCommandId = 0x0200;

        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("b40237da-1c50-4dc7-898d-21c4e08d9b99");

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new TileGeneratorsCommand(package);
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static TileGeneratorsCommand Instance { get; private set; }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes a new instance of the <see cref="UWPTileGeneratorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private TileGeneratorsCommand(Package package)
        {
            if (package == null)
            {
                throw new ArgumentNullException(nameof(package));
            }

            _package = package;

            var commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var splashCommandId = new CommandID(CommandSet, UwpSplashCommandId);
                var splashMenuItem = new MenuCommand(GenerateSplashTiles, splashCommandId);
                commandService.AddCommand(splashMenuItem);

                var tileCommandId = new CommandID(CommandSet, UwpTileCommandId);
                var tileMenuItem = new MenuCommand(GenerateTiles, tileCommandId);
                commandService.AddCommand(tileMenuItem);
            }
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private void GenerateTiles(object sender, EventArgs e)
        {
            var workflow = new GenerateTilesWorkflow(ServiceProvider, ActionType.GenerateSplashImages);
            workflow.DoWork();
        }

        /// <summary>
        /// Generates Splashes screen images.
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        /// <exception cref="Exception">No file was selected</exception>
        private void GenerateSplashTiles(object sender, EventArgs e)
        {
            var workflow = new GenerateTilesWorkflow(ServiceProvider, ActionType.GenerateSplashImages);
            workflow.DoWork();
        }
    }
}
