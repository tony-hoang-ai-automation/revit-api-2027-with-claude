namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Snapshot pure-data của 1 Grid để truyền vào planner.
   ///     Position theo toạ độ feet:
   ///     - Horizontal grid (chạy ngang theo X): Position = Y của grid line
   ///     - Vertical grid (chạy dọc theo Y):   Position = X của grid line
   /// </summary>
   public sealed record GridSnapshot(
      long Id,
      string Name,
      GridAxis Axis,
      double Position);
}
