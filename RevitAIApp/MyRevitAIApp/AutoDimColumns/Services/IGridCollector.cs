using MyRevitAIApp.AutoDimColumns.Models;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Collect Grid elements visible trong view + classify Horizontal/Vertical/Other.
   /// </summary>
   public interface IGridCollector
   {
      CollectResult Collect(Document doc, View view);

      public sealed class CollectResult
      {
         public CollectResult(IReadOnlyList<GridSnapshot> snapshots, IReadOnlyDictionary<long, Grid> gridLookup)
         {
            Snapshots = snapshots;
            GridLookup = gridLookup;
         }

         public IReadOnlyList<GridSnapshot> Snapshots { get; }
         public IReadOnlyDictionary<long, Grid> GridLookup { get; }
      }
   }
}
