using System.Collections.Generic;

namespace MyRevitAIApp.ColumnRebarViewer.Models
{
   /// <summary>
   ///     Mô hình 2D mặt đứng cột để vẽ lên Canvas (mm, gốc chân cột giữa cạnh).
   ///     BarXs = vị trí X các thanh dọc nhìn từ mặt đứng. Stirrups = các lớp đai theo cao độ.
   /// </summary>
   public sealed record RebarElevation2D(
      double WidthMm,
      double HeightMm,
      IReadOnlyList<double> BarXs,
      IReadOnlyList<StirrupLine> Stirrups,
      double ConfinementLengthMm,
      double ConfinementSpacingMm,
      double MidSpacingMm,
      double LneoMm,
      double LnoiMm,
      bool IsValid,
      string? Message);
}
