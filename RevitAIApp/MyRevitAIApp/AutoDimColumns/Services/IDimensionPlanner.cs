using MyRevitAIApp.AutoDimColumns.Models;

namespace MyRevitAIApp.AutoDimColumns.Services
{
   /// <summary>
   ///     Pure-logic planner. Input: snapshots + settings → Output: ordered list of DimSpec.
   ///     KHÔNG phụ thuộc Revit API → unit-testable với xUnit.
   ///
   ///     Trách nhiệm:
   ///     - Skip cột rotation > epsilon (non-orthogonal).
   ///     - Tìm nearest grid X/Y cho mỗi cột.
   ///     - Skip ColumnToGrid khi cột trùng grid (distance &lt; tolerance).
   ///     - Emit self-dim cho cột vuông, radial cho cột tròn.
   ///     - Emit perimeter total dims nếu setting bật.
   /// </summary>
   public interface IDimensionPlanner
   {
      IReadOnlyList<DimSpec> Plan(
         IReadOnlyList<ColumnSnapshot> columns,
         IReadOnlyList<GridSnapshot> grids,
         DimensioningSettings settings);
   }
}
