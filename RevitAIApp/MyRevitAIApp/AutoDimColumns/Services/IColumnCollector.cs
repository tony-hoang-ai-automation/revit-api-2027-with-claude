using MyRevitAIApp.AutoDimColumns.Models;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Collect structural/architectural columns trong active view.
   ///     Output: <see cref="ColumnSnapshot"/> pure-data + dictionary <see cref="FamilyInstance"/>
   ///     để executor lookup lại khi extract Reference.
   /// </summary>
   public interface IColumnCollector
   {
      CollectResult Collect(Document doc, View view, ScopeMode scope, ICollection<ElementId>? selectedIds);

      public sealed class CollectResult
      {
         public CollectResult(IReadOnlyList<ColumnSnapshot> snapshots, IReadOnlyDictionary<long, FamilyInstance> instanceLookup)
         {
            Snapshots = snapshots;
            InstanceLookup = instanceLookup;
         }

         public IReadOnlyList<ColumnSnapshot> Snapshots { get; }
         public IReadOnlyDictionary<long, FamilyInstance> InstanceLookup { get; }
      }
   }
}
