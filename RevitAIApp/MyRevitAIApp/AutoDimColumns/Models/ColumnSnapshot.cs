namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Snapshot pure-data của 1 cột để truyền vào pure-logic planner.
   ///     Không reference đến Revit API type → planner xUnit-testable.
   ///     Đơn vị toạ độ X/Y/Width/Height/Radius: feet (Revit internal unit).
   ///     Rotation: radian, đo từ trục X dương ngược chiều kim đồng hồ.
   /// </summary>
   public sealed record ColumnSnapshot(
      long Id,
      string FamilySymbolName,
      ColumnShape Shape,
      double X,
      double Y,
      double Width,
      double Height,
      double Radius,
      double Rotation);
}
