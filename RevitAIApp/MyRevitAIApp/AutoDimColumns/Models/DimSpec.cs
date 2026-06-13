namespace MyRevitAIApp.AutoDimColumns.Models
{
   /// <summary>
   ///     Spec do planner emit ra, là plan cho 1 dim sẽ được tạo.
   ///     Pure data — không chứa Revit Reference. Executor enrich + tạo dim thật.
   ///
   ///     ColumnId: nullable cho perimeter dim (không gắn 1 cột cụ thể).
   ///     GridIds: 1 grid cho ColumnToGrid; 2+ grid cho PerimeterTotal.
   ///     DimLineOffsetFeet: khoảng cách offset dim line so với face/center của cột.
   ///     SkipReason: nếu set → executor sẽ skip (chỉ hiển thị preview để user thấy).
   /// </summary>
   public sealed record DimSpec(
      DimRequestKind Kind,
      long? ColumnId,
      string ColumnLabel,
      IReadOnlyList<long> GridIds,
      string GridsLabel,
      double DimLineOffsetFeet,
      string? SkipReason);
}
