using System.IO;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using revitProtec.Classes;

namespace revitProtec.Commands
{
    /// <summary>
    ///     External command entry point invoked from the Revit interface
    /// </summary>
    [Transaction(TransactionMode.Manual)]
    public class FlattenSelection : IExternalCommand, IExternalCommandAvailability
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            var app = commandData.Application;
            var uiDoc = app.ActiveUIDocument;
            var doc = uiDoc.Document;
            var activeView = uiDoc.ActiveView;

            var selectedElements = uiDoc.Selection.GetElementIds();


            using (Transaction t = new Transaction(doc, "Isolate Temp"))
            {
                t.Start();
                //temporarily isolate the selection
                activeView.IsolateElementsTemporary(selectedElements);
                t.Commit();
            }
           

            //export the things to DWG
            DWGExportOptions options = new DWGExportOptions
            {
                ExportOfSolids = SolidGeometry.ACIS,
                HideReferencePlane = true,
                HideScopeBox = true,
                HideUnreferenceViewTags = true,
                FileVersion = ACADVersion.Default,
                MergedViews = true
            };

            var fileName = $"protec_{selectedElements.Count} elements from {activeView.Name}";
            var filePath = Path.Combine(Global.TempPath, $"{fileName}.dwg");

            doc.Export(Global.TempPath, fileName,
                new List<ElementId>() { activeView.Id }, options);



            //import the DWG now
            using (Transaction t = new Transaction(doc, "Import the DWG"))
            {
                t.Start();
                doc.Import(filePath, new DWGImportOptions(), activeView, out ElementId newStuff);
                t.Commit();
            }

            //delete the real elements
            using (Transaction t = new Transaction(doc, "Delete the stuff"))
            {
                t.Start();
                doc.Delete(selectedElements);
                t.Commit();
            }

            return Result.Succeeded;
        }

        public bool IsCommandAvailable(UIApplication applicationData, CategorySet selectedCategories)
        {
            var uiDoc = applicationData.ActiveUIDocument;
            var doc = uiDoc.Document;

            return uiDoc.Selection.GetElementIds().Any();
        }
    }

    public class ContextMenuCreator : IContextMenuCreator
    {
        public void BuildContextMenu(ContextMenu menu)
        {
            var commandMenuItem = new CommandMenuItem("Flatten Selection", typeof(FlattenSelection).FullName,
                typeof(ContextMenuApplication).Assembly.Location);

            commandMenuItem.SetAvailabilityClassName(typeof(FlattenSelection).FullName);

            menu.AddItem(new SeparatorItem());

            menu.AddItem(commandMenuItem);
        }
    }
}