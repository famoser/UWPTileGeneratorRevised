# UWP Tile Generator Revised

Generate all UWP tiles needed for UWP applications.  
Recommended workflow: 

 1. Create a quadratic logo with transparent background which spans the entire artboard
 2. Save a version with the foreground color of your choice and one with the foreground color white as .svg file. 
 3. Include the two .svg files into the solution
 4. Richt-click the colored one and select the menu entry "Generate color tiles"
 5. Richt-click the white one and select the menu entry "Generate white tiles"

The tool can resize the source files to all needed resolutions, adding padding as needed. It follows the guidelines provided by microsoft: https://msdn.microsoft.com/windows/uwp/controls-and-patterns/tiles-and-notifications-app-assets   
This is a revised version of https://github.com/shenchauhan/UWPTileGenerator

## Notice for VS 2017 
The functionality of this extension is already included. I still prefer this extension as its faster to use.
https://www.visualstudio.com/en-us/news/releasenotes/vs2017-relnotes#manifest-designer-capability-for-creating-visual-assets

