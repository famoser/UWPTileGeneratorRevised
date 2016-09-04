//------------------------------------------------------------------------------
// <copyright file="TileGeneratorCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using Famoser.UWPTileGeneratorRevised.Enums;
using Famoser.UWPTileGeneratorRevised.Workflow;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace Famoser.UWPTileGeneratorRevised
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class TileGeneratorCommand
    {
        /// <summary>
        /// Command ID.
        /// </summary>
        public const int GenerateAllCommandId = 0x400;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int GenerateTileImagesCommandId = 0x0100;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int GenerateStoreLogoCommandId = 0x0200;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int GenerateSplashImagesCommandId = 0x300;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int GenerateColorImagesCommandId = 0x500;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int GenerateWhiteImagesCommandId = 0x600;

        /// <summary>
        /// Command ID.
        /// </summary>
        public const int ShowHelpCommandId = 0x700;



        /// <summary>
        /// Command menu group (command set GUID).
        /// </summary>
        public static readonly Guid CommandSet = new Guid("145c5f09-ce3c-43f7-9948-18f1ba65f860");

        /// <summary>
        /// VS Package that provides this command, not null.
        /// </summary>
        private readonly Package _package;

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static TileGeneratorCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider => _package;

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package)
        {
            Instance = new TileGeneratorCommand(package);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TileGeneratorCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private TileGeneratorCommand(Package package)
        {
            if (package == null)
                throw new ArgumentNullException(nameof(package));

            _package = package;

            OleMenuCommandService commandService = ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandId = new CommandID(CommandSet, GenerateTileImagesCommandId);
                var menuItem = new MenuCommand(GenerateTileImagesCommandCallback, menuCommandId);
                commandService.AddCommand(menuItem);

                menuCommandId = new CommandID(CommandSet, GenerateSplashImagesCommandId);
                menuItem = new MenuCommand(GenerateSplashImagesCommandCallback, menuCommandId);
                commandService.AddCommand(menuItem);

                menuCommandId = new CommandID(CommandSet, GenerateStoreLogoCommandId);
                menuItem = new MenuCommand(GenerateStoreLogoCommandCallback, menuCommandId);
                commandService.AddCommand(menuItem);

                menuCommandId = new CommandID(CommandSet, GenerateAllCommandId);
                menuItem = new MenuCommand(GenerateAllCommandCallback, menuCommandId);
                commandService.AddCommand(menuItem);

                menuCommandId = new CommandID(CommandSet, GenerateWhiteImagesCommandId);
                menuItem = new MenuCommand(GenerateWhiteImagesCommandCallback, menuCommandId);
                commandService.AddCommand(menuItem);

                menuCommandId = new CommandID(CommandSet, GenerateColorImagesCommandId);
                menuItem = new MenuCommand(GenerateColorImagesCommandCallback, menuCommandId);
                commandService.AddCommand(menuItem);

                menuCommandId = new CommandID(CommandSet, ShowHelpCommandId);
                menuItem = new MenuCommand(ShowHelpCommandCallback, menuCommandId);
                commandService.AddCommand(menuItem);
            }
        }

        private void ShowHelpCommandCallback(object sender, EventArgs e)
        {
            ShowMessage("Help", "Add a .svg or .png file to the project containing the package.manifest file, then this generator will create all images needed for an UWP application. " +
                                "It even configures your package.manifest accordingly, and adds all new files to your project.\n\n" +
                                "Make sure your Logo fulfills folowing requirements: \n" +
                                " - use a quadratic image \n" +
                                " - the background color of the generated image will be evaluated by looking at the pixel (1,1), so use transparent there if you need it\n" +
                                " - use little to no padding for your logo\n\n" +
                                "Recommendations: \n" +
                                " - the background of your logo should be transparent (for the accent color of your customers)\n" +
                                " - make your logo foreground white for tiles, and coloured for store & splash screen");
        }

        private void GenerateColorImagesCommandCallback(object sender, EventArgs e)
        {
            ExecuteWorkflowSafe(ActionType.GenerateStoreLogo, ActionType.GenerateSplashImages);
        }

        private void GenerateWhiteImagesCommandCallback(object sender, EventArgs e)
        {
            ExecuteWorkflowSafe(ActionType.GenerateTileImages);
        }

        private void GenerateAllCommandCallback(object sender, EventArgs e)
        {
            ExecuteWorkflowSafe(ActionType.GenerateStoreLogo, ActionType.GenerateTileImages, ActionType.GenerateSplashImages);
        }

        private void GenerateStoreLogoCommandCallback(object sender, EventArgs e)
        {
            ExecuteWorkflowSafe(ActionType.GenerateStoreLogo);
        }

        private void GenerateSplashImagesCommandCallback(object sender, EventArgs e)
        {
            ExecuteWorkflowSafe(ActionType.GenerateSplashImages);
        }

        private void GenerateTileImagesCommandCallback(object sender, EventArgs e)
        {
            ExecuteWorkflowSafe(ActionType.GenerateTileImages);
        }
        
        private void ExecuteWorkflowSafe(params ActionType[] actions)
        {
            try
            {
                foreach (var actionType in actions)
                {
                    var workflow = new GenerateTilesWorkflow(ServiceProvider, actionType);
                    workflow.DoWork();
                }
                ShowSuccessMessage();
            }
            catch (Exception ex)
            {
                ShowMessage("UPW Tile Generator", "ops! something went wrong: " + ex);
            }
        }

        private void ShowMessage(string title, string message)
        {
            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }

        private void ShowSuccessMessage()
        {
            ShowMessage("UWP Tile Generation", "Generation finished successfull");
        }
    }
}
