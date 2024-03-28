using Autodesk.Revit.UI;
using revitProtec.Commands;

namespace revitProtec
{
    /// <summary>
    ///     Application entry point
    /// </summary>

    public class ContextMenuApplication : IExternalApplication
    {
        public Result OnStartup(UIControlledApplication application)
        {
            // register context menu creator.
            application.RegisterContextMenu(typeof(ContextMenuApplication).FullName, new ContextMenuCreator());
            return Result.Succeeded;
        }

        public Result OnShutdown(UIControlledApplication application)
        {
            return Result.Succeeded;
        }
    }
}