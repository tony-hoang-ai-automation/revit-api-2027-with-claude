using MyRevitAIApp.DuplicateSheet.Models;
using Serilog;

namespace MyRevitAIApp.DuplicateSheet.Services
{
   /// <summary>
   ///     Default <see cref="ISheetDuplicator"/> implementation.
   ///     Caller responsibility: wrap in single <see cref="Transaction"/>, handle rollback.
   ///     Algorithm: xem plan §4.3 + §13.3 (flowchart).
   /// </summary>
   public sealed class SheetDuplicator : ISheetDuplicator
   {
      private readonly ILogger _logger = Log.ForContext<SheetDuplicator>();

      public DuplicationOutcome DuplicateOne(
         Document doc,
         ViewSheet source,
         string newNumber,
         string newName,
         DuplicateMode viewMode,
         SheetContentOptions content)
      {
         try
         {
            var sourceTitleBlock = FindTitleBlockInstance(doc, source);
            var titleBlockTypeId = sourceTitleBlock?.Symbol.Id ?? ElementId.InvalidElementId;

            var newSheet = ViewSheet.Create(doc, titleBlockTypeId);
            newSheet.SheetNumber = newNumber;
            newSheet.Name = newName;

            if (content.CopyTitleBlock && sourceTitleBlock != null)
            {
               AlignTitleBlockPosition(doc, newSheet, sourceTitleBlock);
            }

            var (viewportsCopied, legendsPlaced) = CopyViewports(doc, source, newSheet, viewMode, content.IncludeLegends);

            var schedulesPlaced = content.IncludeSchedules
               ? CopyScheduleInstances(doc, source, newSheet)
               : 0;

            var annotationsCopied = content.CopySheetAnnotations
               ? CopySheetLevelAnnotations(doc, source, newSheet)
               : 0;

            _logger.Information(
               "Duplicated sheet {SrcNumber} → {NewNumber} ({NewName}). Viewports={Vps}, Legends={Lgs}, Schedules={Schs}, Annotations={Ann}",
               source.SheetNumber, newSheet.SheetNumber, newSheet.Name,
               viewportsCopied, legendsPlaced, schedulesPlaced, annotationsCopied);

            return new DuplicationOutcome(
               source.Id, newSheet.Id,
               newSheet.SheetNumber, newSheet.Name,
               viewportsCopied, legendsPlaced, schedulesPlaced, annotationsCopied,
               true, null);
         }
         catch (Exception ex)
         {
            _logger.Error(ex, "Failed to duplicate sheet {SrcNumber}", source.SheetNumber);
            return DuplicationOutcome.Failed(source.Id, newNumber, newName, ex.Message);
         }
      }

      private static FamilyInstance? FindTitleBlockInstance(Document doc, ViewSheet sheet) =>
         new FilteredElementCollector(doc, sheet.Id)
            .OfClass(typeof(FamilyInstance))
            .OfCategory(BuiltInCategory.OST_TitleBlocks)
            .WhereElementIsNotElementType()
            .FirstOrDefault() as FamilyInstance;

      private static void AlignTitleBlockPosition(Document doc, ViewSheet newSheet, FamilyInstance sourceTitleBlock)
      {
         var newTitleBlock = FindTitleBlockInstance(doc, newSheet);
         if (newTitleBlock == null) return;
         if (sourceTitleBlock.Location is not LocationPoint srcLoc) return;
         if (newTitleBlock.Location is not LocationPoint newLoc) return;

         var delta = srcLoc.Point - newLoc.Point;
         if (delta.IsZeroLength()) return;

         ElementTransformUtils.MoveElement(doc, newTitleBlock.Id, delta);
      }

      private (int viewports, int legends) CopyViewports(
         Document doc,
         ViewSheet source,
         ViewSheet target,
         DuplicateMode viewMode,
         bool includeLegends)
      {
         var viewportsCopied = 0;
         var legendsPlaced = 0;

         foreach (var vpId in source.GetAllViewports())
         {
            if (doc.GetElement(vpId) is not Viewport vp) continue;
            if (doc.GetElement(vp.ViewId) is not View view) continue;

            var position = vp.GetBoxCenter();

            if (view.ViewType == ViewType.Legend)
            {
               if (includeLegends)
               {
                  Viewport.Create(doc, target.Id, view.Id, position);
                  legendsPlaced++;
               }
               continue;
            }

            var option = MapToRevitOption(viewMode);
            if (!view.CanViewBeDuplicated(option))
            {
               _logger.Warning(
                  "Skip viewport: view '{Name}' (type {Type}) cannot be duplicated with option {Option}",
                  view.Name, view.ViewType, option);
               continue;
            }

            var newViewId = view.Duplicate(option);
            Viewport.Create(doc, target.Id, newViewId, position);
            viewportsCopied++;
         }

         return (viewportsCopied, legendsPlaced);
      }

      private static int CopyScheduleInstances(Document doc, ViewSheet source, ViewSheet target)
      {
         var instances = new FilteredElementCollector(doc, source.Id)
            .OfClass(typeof(ScheduleSheetInstance))
            .Cast<ScheduleSheetInstance>()
            .Where(s => !s.IsTitleblockRevisionSchedule)
            .ToList();

         var count = 0;
         foreach (var ssi in instances)
         {
            ScheduleSheetInstance.Create(doc, target.Id, ssi.ScheduleId, ssi.Point);
            count++;
         }
         return count;
      }

      private static int CopySheetLevelAnnotations(Document doc, ViewSheet source, ViewSheet target)
      {
         var ids = new FilteredElementCollector(doc)
            .WherePasses(new ElementOwnerViewFilter(source.Id))
            .WhereElementIsNotElementType()
            .Where(IsCopyableSheetAnnotation)
            .Select(e => e.Id)
            .ToList();

         if (ids.Count == 0) return 0;

         ElementTransformUtils.CopyElements(source, ids, target, Transform.Identity, new CopyPasteOptions());
         return ids.Count;
      }

      private static bool IsCopyableSheetAnnotation(Element e) =>
         e is TextNote
         || e is Dimension
         || e is DetailCurve
         || e is FilledRegion
         || e is RevisionCloud;

      private static ViewDuplicateOption MapToRevitOption(DuplicateMode mode) => mode switch
      {
         DuplicateMode.Duplicate => ViewDuplicateOption.Duplicate,
         DuplicateMode.WithDetailing => ViewDuplicateOption.WithDetailing,
         DuplicateMode.AsDependent => ViewDuplicateOption.AsDependent,
         _ => ViewDuplicateOption.Duplicate
      };
   }
}
