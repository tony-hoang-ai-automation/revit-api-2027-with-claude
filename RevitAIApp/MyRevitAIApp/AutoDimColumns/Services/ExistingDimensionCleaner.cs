using MyRevitAIApp.AutoDimColumns.Internals;
using Serilog;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Default <see cref="IExistingDimensionCleaner"/>. Match qua
   ///     <c>dim.References[i].ElementId</c> ↔ target column IDs.
   /// </summary>
   public sealed class ExistingDimensionCleaner : IExistingDimensionCleaner
   {
      private readonly ILogger _logger = Log.ForContext<ExistingDimensionCleaner>();

      public int CountOverlapping(Document doc, View view, IEnumerable<long> targetColumnIds)
      {
         var targets = new HashSet<long>(targetColumnIds);
         if (targets.Count == 0) return 0;

         return FindOverlappingIds(doc, view, targets).Count;
      }

      public int DeleteOverlapping(Document doc, View view, IEnumerable<long> targetColumnIds)
      {
         var targets = new HashSet<long>(targetColumnIds);
         if (targets.Count == 0) return 0;

         var idsToDelete = FindOverlappingIds(doc, view, targets);
         if (idsToDelete.Count == 0) return 0;

         doc.Delete(idsToDelete);
         _logger.Information("Deleted {Count} existing dimensions overlapping target columns in view {View}",
            idsToDelete.Count, view.Name);
         return idsToDelete.Count;
      }

      private static List<ElementId> FindOverlappingIds(Document doc, View view, HashSet<long> targets)
      {
         var ids = new List<ElementId>();
         var dims = new FilteredElementCollector(doc, view.Id)
            .OfClass(typeof(Dimension))
            .Cast<Dimension>();

         foreach (var dim in dims)
         {
            foreach (Reference r in dim.References)
            {
               if (r.ElementId == ElementId.InvalidElementId) continue;
               if (targets.Contains(r.ElementId.GetIdValue()))
               {
                  ids.Add(dim.Id);
                  break;
               }
            }
         }

         return ids;
      }
   }
}
