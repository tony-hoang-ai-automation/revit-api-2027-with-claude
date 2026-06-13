using MyRevitAIApp.AutoDimColumns.Models;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Orchestrator: enrich DimSpec với Reference thật + call NewDimension / NewRadialDimension.
   ///     Caller phải wrap call trong <see cref="Transaction"/>.
   ///     Resilient: try/catch từng spec, không abort cả batch khi 1 dim fail.
   /// </summary>
   public interface IDimensionExecutor
   {
      DimensioningOutcome Execute(
         Document doc,
         View view,
         IReadOnlyList<DimSpec> specs,
         DimensioningSettings settings,
         IReadOnlyDictionary<long, FamilyInstance> columnIndex,
         IReadOnlyDictionary<long, Grid> gridIndex,
         DimensionType linearDimType,
         DimensionType radialDimType);
   }
}
