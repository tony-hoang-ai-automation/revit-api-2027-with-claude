namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     1 vùng rải đai theo cao độ (để tạo Rebar bằng layout Number+Spacing trong Revit).
   ///     FirstZMm = cao độ đai đầu tiên của vùng (tính từ chân cột); Count = số đai;
   ///     SpacingMm = bước đai; IsConfinement = vùng gia cường đầu cột.
   /// </summary>
   public readonly record struct StirrupZone(
      double FirstZMm,
      int Count,
      double SpacingMm,
      bool IsConfinement);
}
