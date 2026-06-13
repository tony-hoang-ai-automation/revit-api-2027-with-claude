using System.Collections.Generic;

namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Mô hình 2D mặt cắt cột để vẽ lên Canvas (mm, gốc tâm tiết diện).
   ///     IsValid=false khi input không hợp lệ (cover quá lớn, n&lt;4...) — chỉ vẽ outline.
   ///     CrossTies = các đai phụ chữ C người dùng đã thêm (click thanh thép dọc).
   /// </summary>
   public sealed record RebarSection2D(
      double BMm,
      double HMm,
      double CoverMm,
      double StirrupDiameterMm,
      IReadOnlyList<BarPoint> Bars,
      bool IsValid,
      string? Message,
      IReadOnlyList<CrossTie> CrossTies);
}
